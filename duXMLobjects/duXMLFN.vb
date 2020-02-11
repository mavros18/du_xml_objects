Imports System.Runtime.CompilerServices

Imports duXML.Objects
Imports System
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Text.RegularExpressions


Namespace Functions

#Region "Functions"

    Public Class duXML_parser
        'Inherits _SessionTree

        Private mytmp As List(Of _SessionTree)

        'Private Sub duXML_parser()
        'End Sub

        Private Function getDirectChild(ByVal parent As XmlElement, ByVal name As String) As XmlElement
            Dim child As XmlNode = parent.FirstChild
            Do While child IsNot Nothing
                If TypeOf child Is XmlElement AndAlso name.Equals(child.Name.ToString) Then
                    Return CType(child, XmlElement)
                End If
                child = child.NextSibling
            Loop
            Return Nothing
        End Function

        Private Sub rec_tree(ByVal myroot As XmlElement, ByVal parentmu As String, ByVal myparent As String, ByVal myparentID As Integer, ByVal con_type As String)


            If myroot Is Nothing Then
                Console.WriteLine("NULL DETECTED")
            End If

            Dim tempE As XmlElement
            Dim mymu, myname As String
            tempE = getDirectChild(myroot, "data")


            myname = tempE.GetElementsByTagName("uprocName").Item(0).InnerXml

            If "MU".Equals(tempE.GetElementsByTagName("type").Item(0).InnerXml) Then
                mymu = tempE.GetElementsByTagName("muName").Item(0).InnerXml
            Else
                mymu = parentmu
            End If

            mytmp.Add(New _SessionTree().sessionTree(myname, tempE.GetElementsByTagName("type").Item(0).InnerXml, mymu, myparent, myparentID, con_type))

            Dim mid As Integer = mytmp.Count - 1

            tempE = getDirectChild(myroot, "childOk")
            If tempE IsNot Nothing Then
                rec_tree(tempE, mymu, myname, mid, "childOk")
            End If

            tempE = getDirectChild(myroot, "childKo")
            If tempE IsNot Nothing Then
                rec_tree(tempE, mymu, myname, mid, "childKo")
            End If

            tempE = getDirectChild(myroot, "nextSibling")
            If tempE IsNot Nothing Then
                rec_tree(tempE, parentmu, myparent, myparentID, "nextSibling")
            End If
        End Sub

        Public Function get_by_name(Of T As _baseObject)(ByVal list_of_objects As List(Of T), ByVal search_name As String, ByVal case_sensitive As Boolean?) As T
            If case_sensitive Then
                For Each obj As T In list_of_objects
                    If search_name.Equals(obj.name) Then
                        Return obj
                    End If
                Next obj
            Else
                Dim name As String = search_name.ToUpper()
                For Each obj As T In list_of_objects
                    If name.Equals(obj.name.ToUpper()) Then
                        Return obj
                    End If
                Next obj
            End If
            Return Nothing
        End Function

        Public Function run(ByVal path As String, ByVal nodename_regex As String) As List(Of _Node)

            Dim du As New List(Of _Node)

            Dim task_vars As Dictionary(Of String, String)
            Dim ss As List(Of _Session)
            Dim mm As List(Of _Mu)
            Dim tt As List(Of _Task)
            Dim rr As List(Of _Resources)
            Dim uu As List(Of _Uproc)
            Dim nodename, tmp, task_session, task_uproc, task_is_template, task_type, task_mu, task_active, new_l, tt_queue, tt_optional, t_name As String
            Dim task_launch As List(Of String)
            Dim tt_rule As List(Of String)
            Dim utmp As _Uproc
            Dim cur_dep As _Dependencies

            Dim p As Regex = New Regex(nodename_regex, RegexOptions.IgnoreCase)


            Dim listOfFiles As String() = Directory.GetFiles(path)
            For Each fileName As String In listOfFiles
                Dim currentFile As FileInfo = New FileInfo(fileName)
                If (currentFile.Exists) And (currentFile.Name.EndsWith("xml", StringComparison.OrdinalIgnoreCase)) Then

                    ss = New List(Of _Session)()
                    mm = New List(Of _Mu)()
                    tt = New List(Of _Task)()
                    uu = New List(Of _Uproc)()
                    rr = New List(Of _Resources)()


                    Try
                        Console.WriteLine("Start parsing file """ & currentFile.Name & """ ...")

                        Dim m As Match = p.Match(currentFile.Name)
                        If Not m.Success Then
                            Console.WriteLine("No match for nodename!")
                        Else
                            Dim key As String = m.Groups(1).Value
                            nodename = m.Groups(1).Value

                            Console.WriteLine("Nodename : " + nodename)

                            Dim doc As New XmlDocument
                            Dim xmlSerializer As XmlSerializer = New XmlSerializer(doc.GetType)
                            Dim readStream As FileStream = New FileStream(currentFile.FullName, FileMode.Open)
                            doc = CType(xmlSerializer.Deserialize(readStream), XmlDocument)
                            readStream.Close()



                            Dim nPar, nNode, nNode2, tempNode As XmlNode
                            Dim nList, nList2, nList3 As XmlNodeList
                            Dim myElem, eElement, eElement2, tempElem, tempElem2 As XmlElement

                            nPar = doc.GetElementsByTagName("mus").Item(0)
                            myElem = CType(nPar, XmlElement)
                            nList = myElem.GetElementsByTagName("com.orsyp.ExportMu")
                            For temp As Integer = 0 To nList.Count - 1
                                nNode = nList.Item(temp)
                                If nNode.NodeType = XmlNodeType.Element Then
                                    eElement = CType(nNode, XmlElement)
                                    eElement2 = getDirectChild(eElement, "mu")
                                    eElement2 = getDirectChild(eElement2, "identifier")
                                    mm.Add(New _Mu().mu(eElement2.GetElementsByTagName("name").Item(0).InnerXml, eElement.GetElementsByTagName("nodeName").Item(0).InnerXml))
                                End If
                            Next temp

                            nPar = doc.GetElementsByTagName("com.orsyp.ExportObjects").Item(0)
                            myElem = getDirectChild(CType(nPar, XmlElement), "resources")
                            nList = myElem.GetElementsByTagName("Resource")
                            For temp As Integer = 0 To nList.Count - 1
                                nNode = nList.Item(temp)
                                If nNode.NodeType = XmlNodeType.Element Then
                                    eElement = CType(nNode, XmlElement)
                                    eElement2 = getDirectChild(eElement, "identifier")
                                    rr.Add(New _Resources().resource(eElement2.GetElementsByTagName("name").Item(0).InnerXml, eElement.GetElementsByTagName("nature").Item(0).InnerXml))
                                End If
                            Next temp

                            nPar = doc.GetElementsByTagName("sessions").Item(0)
                            myElem = CType(nPar, XmlElement)
                            nList = myElem.GetElementsByTagName("Session")
                            For temp As Integer = 0 To nList.Count - 1
                                nNode = nList.Item(temp)
                                If nNode.NodeType = XmlNodeType.Element Then
                                    eElement = CType(nNode, XmlElement)
                                    eElement2 = getDirectChild(eElement, "identifier")
                                    tmp = eElement2.GetElementsByTagName("name").Item(0).InnerXml
                                    eElement2 = getDirectChild(eElement, "tree")
                                    mytmp = New List(Of _SessionTree)
                                    tempElem = getDirectChild(eElement2, "root")
                                    rec_tree(tempElem, "", "", -1, "")
                                    eElement2 = getDirectChild(eElement, "label")

                                    ss.Add(New _Session().session(tmp, mytmp, eElement2.InnerXml))
                                End If
                            Next temp
                            mytmp = Nothing

                            nPar = doc.GetElementsByTagName("tasks").Item(0)
                            myElem = CType(nPar, XmlElement)
                            nList = myElem.GetElementsByTagName("Task")
                            For temp As Integer = 0 To nList.Count - 1
                                nNode = nList.Item(temp)
                                If nNode.NodeType = XmlNodeType.Element Then
                                    eElement = CType(nNode, XmlElement)
                                    eElement2 = getDirectChild(eElement, "identifier")

                                    If eElement2.GetElementsByTagName("name").Count = 0 Then
                                        t_name = ""
                                    Else
                                        t_name = eElement2.GetElementsByTagName("name").Item(0).InnerXml
                                    End If

                                    task_session = eElement2.GetElementsByTagName("sessionName").Item(0).InnerXml
                                    task_uproc = eElement2.GetElementsByTagName("uprocName").Item(0).InnerXml
                                    task_is_template = eElement.GetElementsByTagName("template").Item(0).InnerXml
                                    task_type = getDirectChild(eElement, "specificData").GetAttribute("class")
                                    task_mu = eElement2.GetElementsByTagName("muName").Item(0).InnerXml
                                    task_active = CBool(getDirectChild(eElement, "active").InnerXml)
                                    tt_queue = getDirectChild(eElement, "queue").InnerXml
                                    task_vars = New Dictionary(Of String, String)

                                    tt_optional = "false"
                                    tt_rule = New List(Of String)
                                    task_launch = New List(Of String)

                                    If "TaskPlanified".Equals(task_type) Then
                                        tempElem = getDirectChild(eElement, "specificData")
                                        tt_optional = getDirectChild(tempElem, "optional").InnerXml
                                        nList2 = tempElem.GetElementsByTagName("com.orsyp.api.task.TaskImplicitData")
                                        For j As Integer = 0 To nList2.Count - 1
                                            nNode2 = nList2.Item(j)
                                            tempElem2 = getDirectChild(CType(nNode2, XmlElement), "identifier")
                                            tt_rule.Add(getDirectChild(tempElem2, "name").InnerXml)
                                        Next j

                                        If ("false".Equals(task_is_template)) Then
                                            Dim patterns As XmlElement = getDirectChild(tempElem, "launchHourPatterns") ' DU 6
                                            Dim relaunch As XmlElement = getDirectChild(tempElem, "autoRelaunch") ' DU 6
                                            nList2 = eElement.GetElementsByTagName("launchTime") ' embeddedLaunchTime DU 6 || simpleLaunches om.orsyp.api.task.SimpleLaunch DU 5
                                            nList3 = eElement.GetElementsByTagName("launchTimes") ' multipleLaunch DU 5

                                            If patterns IsNot Nothing Then
                                                nList2 = patterns.GetElementsByTagName("startTime")
                                                For j As Integer = 0 To nList2.Count - 1
                                                    nNode2 = nList2.Item(j)
                                                    If Not task_launch.Contains(nNode2.InnerXml.Replace(" ", "0")) Then
                                                        task_launch.Add(nNode2.InnerXml.Replace(" ", "0"))
                                                    End If
                                                Next j
                                            ElseIf relaunch IsNot Nothing Then
                                                tempElem2 = getDirectChild(relaunch, "from")
                                                Dim tth, ttm As String
                                                tth = getDirectChild(tempElem2, "hour").InnerXml
                                                If tth.Length = 1 Then
                                                    tth = "0" & tth
                                                End If
                                                ttm = getDirectChild(tempElem2, "minute").InnerXml
                                                If ttm.Length = 1 Then
                                                    ttm = "0" & ttm
                                                End If
                                                task_launch.Add(tth & ttm & "00")
                                            ElseIf nList2.Count > 0 Then
                                                For j As Integer = 0 To nList2.Count - 1
                                                    nNode2 = nList2.Item(j)
                                                    If Not task_launch.Contains(nNode2.InnerXml) Then
                                                        task_launch.Add(nNode2.InnerXml)
                                                    End If
                                                Next j
                                            ElseIf nList3.Count > 0 Then
                                                nList2 = CType(eElement.GetElementsByTagName("launchTimes").Item(0), XmlElement).GetElementsByTagName("string")
                                                For j As Integer = 0 To nList2.Count - 1
                                                    nNode2 = nList2.Item(j)
                                                    new_l = nNode2.InnerXml & "00"

                                                    If Not task_launch.Contains(new_l) Then
                                                        task_launch.Add(new_l)
                                                    End If
                                                Next j
                                            Else
                                                'utility.out.println("No launches : "+t_name);
                                            End If

                                        End If

                                    ElseIf "TaskProvoked".Equals(task_type) Then
                                        If ("false".Equals(task_is_template)) Then
                                            tempElem = getDirectChild(eElement, "specificData")
                                            tempElem2 = getDirectChild(tempElem, "startLaunchTime")
                                            If Not "999999".Equals(tempElem2.InnerXml) Then
                                                task_launch.Add(tempElem2.InnerXml)
                                            End If
                                        End If
                                    ElseIf Not "TaskCyclic".Equals(task_type) Then
                                        Console.WriteLine(nodename & " [" & task_session & "," & task_uproc & "] : [" & task_type & "] : INVALID_TYPE")
                                    End If

                                    myElem = getDirectChild(eElement, "variables")
                                    If myElem IsNot Nothing Then
                                        nList2 = myElem.GetElementsByTagName("com.orsyp.api.VariableText")
                                        For j As Integer = 0 To nList2.Count - 1
                                            nNode2 = nList2.Item(j)
                                            eElement2 = CType(nNode2, XmlElement)
                                            If eElement2.GetElementsByTagName("name").Count > 0 Then
                                                tempElem = getDirectChild(eElement2, "value")
                                                task_vars.Add(eElement2.GetElementsByTagName("name").Item(0).InnerXml, tempElem.InnerXml)
                                            End If
                                        Next j

                                        nList2 = myElem.GetElementsByTagName("com.orsyp.api.VariableNumeric")
                                        For j As Integer = 0 To nList2.Count - 1
                                            nNode2 = nList2.Item(j)
                                            eElement2 = CType(nNode2, XmlElement)
                                            If eElement2.GetElementsByTagName("name").Count > 0 Then
                                                tempElem = getDirectChild(eElement2, "value")
                                                task_vars.Add(eElement2.GetElementsByTagName("name").Item(0).InnerXml, tempElem.InnerXml)
                                            End If
                                        Next j
                                    End If

                                    tt.Add(New _Task().task(task_session, task_uproc, task_launch, task_active, task_mu, task_is_template, task_type, _
                                                            eElement.GetElementsByTagName("isUprocHeader").Item(0).InnerXml, task_vars, tt_queue, tt_rule, tt_optional, t_name))
                                End If
                            Next temp

                            nPar = doc.GetElementsByTagName("uprocs").Item(0)
                            myElem = CType(nPar, XmlElement)
                            nList = myElem.GetElementsByTagName("com.orsyp.ExportUproc")
                            For temp As Integer = 0 To nList.Count - 1
                                nNode = nList.Item(temp)
                                If nNode.NodeType = XmlNodeType.Element Then
                                    utmp = New _Uproc

                                    eElement = CType(nNode, XmlElement)
                                    eElement = getDirectChild(eElement, "uproc")

                                    'name
                                    eElement2 = getDirectChild(eElement, "identifier")
                                    tmp = eElement2.GetElementsByTagName("name").Item(0).InnerXml
                                    utmp.name = tmp

                                    'class
                                    eElement2 = getDirectChild(eElement, "uprocClass")
                                    utmp.mClass = eElement2.InnerXml

                                    'incompatibilities
                                    myElem = getDirectChild(eElement, "incompatibilities")
                                    If myElem IsNot Nothing Then

                                        nList2 = myElem.GetElementsByTagName("com.orsyp.api.uproc.Incompatibility")
                                        For j As Integer = 0 To nList2.Count - 1
                                            nNode2 = nList2.Item(j)
                                            eElement2 = CType(nNode2, XmlElement)
                                            If eElement2.GetElementsByTagName("uprocClass").Count > 0 Then
                                                utmp.mIncomp.Add(eElement2.GetElementsByTagName("uprocClass").Item(0).InnerXml)
                                            End If
                                        Next j
                                    End If

                                    'successors
                                    myElem = getDirectChild(eElement, "successors")
                                    If myElem IsNot Nothing Then

                                        nList2 = myElem.GetElementsByTagName("Successor")
                                        For j As Integer = 0 To nList2.Count - 1
                                            nNode2 = nList2.Item(j)
                                            eElement2 = CType(nNode2, XmlElement)
                                            If eElement2.GetElementsByTagName("name").Count > 0 Then
                                                utmp.mSuccessors.Add(eElement2.GetElementsByTagName("name").Item(0).InnerXml)
                                            End If
                                        Next j

                                    End If

                                    'resources
                                    myElem = getDirectChild(eElement, "resources")
                                    If myElem IsNot Nothing Then

                                        nList2 = myElem.GetElementsByTagName("com.orsyp.api.uproc.ResourceCondition")
                                        For j As Integer = 0 To nList2.Count - 1
                                            nNode2 = nList2.Item(j)
                                            eElement2 = CType(nNode2, XmlElement)
                                            If eElement2.GetElementsByTagName("resource").Count > 0 Then
                                                tempElem = getDirectChild(eElement2, "type")
                                                utmp.mResources.Add(New _Resources().resource(eElement2.GetElementsByTagName("resource").Item(0).InnerXml, tempElem.InnerXml))
                                            End If
                                        Next j

                                    End If

                                    'variables
                                    myElem = getDirectChild(eElement, "variables")
                                    If myElem IsNot Nothing Then

                                        nList2 = myElem.GetElementsByTagName("com.orsyp.api.VariableText")
                                        For j As Integer = 0 To nList2.Count - 1
                                            nNode2 = nList2.Item(j)
                                            eElement2 = CType(nNode2, XmlElement)
                                            If eElement2.GetElementsByTagName("name").Count > 0 Then
                                                tempElem = getDirectChild(eElement2, "value")
                                                utmp.mVariables_Add(eElement2.GetElementsByTagName("name").Item(0).InnerXml, tempElem.InnerXml)
                                            End If
                                        Next j

                                        nList2 = myElem.GetElementsByTagName("com.orsyp.api.VariableNumeric")
                                        For j As Integer = 0 To nList2.Count - 1
                                            nNode2 = nList2.Item(j)
                                            eElement2 = CType(nNode2, XmlElement)
                                            If eElement2.GetElementsByTagName("name").Count > 0 Then
                                                tempElem = getDirectChild(eElement2, "value")
                                                utmp.mVariables_Add(eElement2.GetElementsByTagName("name").Item(0).InnerXml, tempElem.InnerXml)
                                            End If
                                        Next j

                                    End If

                                    'nonSimultaneities
                                    nList2 = eElement.GetElementsByTagName("com.orsyp.api.uproc.NonSimultaneityCondition")
                                    For j As Integer = 0 To nList2.Count - 1
                                        nNode2 = nList2.Item(j)
                                        eElement2 = CType(nNode2, XmlElement)
                                        If eElement2.GetElementsByTagName("uproc").Count > 0 Then

                                            cur_dep = New _Dependencies()

                                            Debug.Assert(eElement2.GetElementsByTagName("uproc").Count = 1)

                                            cur_dep.mUproc = eElement2.GetElementsByTagName("uproc").Item(0).InnerXml

                                            Debug.Assert(eElement2.GetElementsByTagName("sessionControl").Count = 1)
                                            tempNode = eElement2.GetElementsByTagName("sessionControl").Item(0)
                                            tempElem = CType(tempNode, XmlElement)

                                            If "SPECIFIC_SESSION".Equals(tempElem.GetElementsByTagName("type").Item(0).InnerXml) Then
                                                cur_dep.mSession = tempElem.GetElementsByTagName("session").Item(0).InnerXml
                                            Else
                                                cur_dep.mSession = tempElem.GetElementsByTagName("type").Item(0).InnerXml
                                            End If

                                            Debug.Assert(eElement2.GetElementsByTagName("muControl").Count = 1)
                                            tempNode = eElement2.GetElementsByTagName("muControl").Item(0)
                                            tempElem = CType(tempNode, XmlElement)

                                            If "SPECIFIC_MU".Equals(tempElem.GetElementsByTagName("type").Item(0).InnerXml) Then
                                                cur_dep.mMU = tempElem.GetElementsByTagName("mu").Item(0).InnerXml
                                            Else
                                                cur_dep.mMU = tempElem.GetElementsByTagName("type").Item(0).InnerXml
                                            End If

                                            utmp.mNotSim.Add(cur_dep)

                                        End If
                                    Next j

                                    'dependencies
                                    nList2 = eElement.GetElementsByTagName("com.orsyp.api.uproc.DependencyCondition")
                                    For j As Integer = 0 To nList2.Count - 1
                                        nNode2 = nList2.Item(j)
                                        eElement2 = CType(nNode2, XmlElement)
                                        If eElement2.GetElementsByTagName("uproc").Count > 0 Then
                                            cur_dep = New _Dependencies()

                                            Debug.Assert(eElement2.GetElementsByTagName("uproc").Count = 1)

                                            cur_dep.mUproc = eElement2.GetElementsByTagName("uproc").Item(0).InnerXml

                                            Debug.Assert(eElement2.GetElementsByTagName("sessionControl").Count = 1)
                                            tempNode = eElement2.GetElementsByTagName("sessionControl").Item(0)
                                            tempElem = CType(tempNode, XmlElement)

                                            If "SPECIFIC_SESSION".Equals(tempElem.GetElementsByTagName("type").Item(0).InnerXml) Then
                                                cur_dep.mSession = tempElem.GetElementsByTagName("session").Item(0).InnerXml
                                            Else
                                                cur_dep.mSession = tempElem.GetElementsByTagName("type").Item(0).InnerXml
                                            End If

                                            Debug.Assert(eElement2.GetElementsByTagName("muControl").Count = 1)
                                            tempNode = eElement2.GetElementsByTagName("muControl").Item(0)
                                            tempElem = CType(tempNode, XmlElement)

                                            If tempElem.GetElementsByTagName("type").Item(0) IsNot Nothing Then
                                                If "SPECIFIC_MU".Equals(tempElem.GetElementsByTagName("type").Item(0).InnerXml) Then
                                                    cur_dep.mMU = tempElem.GetElementsByTagName("mu").Item(0).InnerXml
                                                Else
                                                    cur_dep.mMU = tempElem.GetElementsByTagName("type").Item(0).InnerXml
                                                End If
                                            ElseIf tempElem.GetElementsByTagName("mu").Item(0) IsNot Nothing Then
                                                cur_dep.mMU = tempElem.GetElementsByTagName("mu").Item(0).InnerXml
                                                Console.Error.WriteLine("Warning: " & tmp & " : missing <type> on <muControl> on <com.orsyp.api.uproc.DependencyCondition> : Resolved to " & tempElem.GetElementsByTagName("mu").Item(0).InnerXml & "")
                                            Else
                                                cur_dep.mMU = "!ERROR_MISSING!"
                                                Console.Error.WriteLine("ERROR: " & tmp & " : missing <type> on <muControl> on <com.orsyp.api.uproc.DependencyCondition>")
                                            End If

                                            utmp.mDependencies_Add(cur_dep)
                                            'utmp.mDependencies.Add(cur_dep)
                                        End If
                                    Next j

                                    'internalScript
                                    myElem = getDirectChild(eElement, "type")
                                    If "CL_INT".Equals(myElem.InnerXml) Then

                                        myElem = getDirectChild(eElement, "internalScript")
                                        If myElem IsNot Nothing Then

                                            eElement2 = getDirectChild(myElem, "lines")
                                            nList2 = eElement2.GetElementsByTagName("string")

                                            For j As Integer = 0 To nList2.Count - 1
                                                nNode2 = nList2.Item(j)
                                                utmp.mIS.Add(nNode2.InnerXml)
                                            Next j
                                        Else
                                            Console.WriteLine(tmp & " : TYPE CL_INT doesn't have internal script")
                                        End If
                                    End If

                                    uu.Add(utmp)

                                End If
                            Next temp
                            du.Add(New _Node().node(nodename, ss, mm, tt, uu, rr))
                            Console.WriteLine("Finished parsing!" + vbCrLf)

                        End If
                    Catch ex As Exception
                        Console.WriteLine(ex)
                        'Throw ex
                    End Try

                End If


            Next

            Return du
        End Function

    End Class

#End Region
End Namespace