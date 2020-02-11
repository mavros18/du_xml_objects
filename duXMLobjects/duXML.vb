Imports System.Runtime.CompilerServices

Imports System
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Namespace Objects

#Region "Objects"

#Region "_baseObject Class"
    Public Class _baseObject
        Private _name As String
        Private _title As String

        Property name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Property title() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
            End Set
        End Property

        Public Overrides Function toString() As String
            Return "[name: " + _name + ", title:" + _title + "]"
        End Function

        Public Overridable Function toJSONstring() As String
            Return "{""name"":""" + Me.name + """, ""title"":""" + _title + """}"
        End Function

    End Class
#End Region

#Region "_Dependencies Class_DONE"
    Public Class _Dependencies
        Private _uproc As String
        Private _session As String
        Private _mu As String
        Private _node As String
        Private _description As String

        Property mUproc As String
            Get
                Return _uproc
            End Get
            Set(ByVal value As String)
                _uproc = value
            End Set
        End Property

        Property mSession As String
            Get
                Return _session
            End Get
            Set(ByVal value As String)
                _session = value
            End Set
        End Property

        Property mMU As String
            Get
                Return _mu
            End Get
            Set(ByVal value As String)
                _mu = value
            End Set
        End Property

        Property mNode As String
            Get
                Return _node
            End Get
            Set(ByVal value As String)
                _node = value
            End Set
        End Property

        Property mDescription As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Public Sub dependency()
            With Me
                ._uproc = ""
                ._session = ""
                ._mu = ""
                ._node = ""
                ._description = ""
            End With
        End Sub

        Public Sub dependency(ByVal uproc As String, ByVal any_session As String, ByVal mu As String, ByVal node As String, ByVal classname As String)
            With Me
                ._uproc = uproc
                ._session = any_session
                ._mu = mu
                ._node = node
                ._description = classname
            End With
        End Sub

        Public Overrides Function ToString() As String
            With Me
                If ._description.Equals("") Then
                    Return "uproc:" + ._uproc + ", session:" + ._session + ", mu:" + ._mu + ", node:" + ._node + "]"
                End If
                Return "uproc:" + ._uproc + ", session:" + ._session + ", mu:" + ._mu + ", node:" + ._node + ", " + ._description + "]"
            End With
        End Function

        Public Function ToJSONstring() As String
            With Me
                If ._description.Equals("") Then
                    Return "{""uproc"":""" + ._uproc + """, ""session"":""" + ._session + """, ""mu"":""" + ._mu + """, ""node"":""" + ._node + """}"
                End If
                Return "{""uproc"":""" + ._uproc + """, ""session"":""" + ._session + """, ""mu"":""" + ._mu + """, ""node"":""" + ._node + """, ""description"":""" + Me._description + """}"
            End With
        End Function
    End Class
#End Region

#Region "_Resources Class_DONE"
    Public Class _Resources
        Inherits _baseObject

        Private _type As String

        Public Function resource(ByVal name As String, ByVal type As String)
            With Me
                .name = name
                ._type = type
            End With
            Return Me
        End Function

        Public Overrides Function toString() As String
            Return "[resource:" + Me.name + ", type:" + _type + "]"
        End Function

        Public Function toHumanString() As String
            Return "Resource: " + Me.name + "- Type: " + _type + "]"
        End Function

        Public Overrides Function toJSONstring() As String
            Return "{""resource"":""" + Me.name + """, ""type"":""" + _type + """}"
        End Function

    End Class
#End Region

#Region "_Uproc Class_DONE"
    Public Class _Uproc
        Inherits _baseObject

        Private _notsim As New List(Of _Dependencies)
        Private _dependencies As New List(Of _Dependencies)
        Private _IS As New List(Of String)
        Private _class As String
        Private _incomp As New List(Of String)
        Private _successors As New List(Of String)
        Private _resources As New List(Of _Resources)
        Private _variables As New Dictionary(Of String, String)

        ReadOnly Property mNotSim As List(Of _Dependencies)
            Get
                Return _notsim
            End Get
        End Property

        Public Sub mNotSim_Add(ByVal mNotSim As _Dependencies)
            _notsim.Add(mNotSim)
        End Sub

        ReadOnly Property mDependencies As List(Of _Dependencies)
            Get
                Return _dependencies
            End Get
        End Property

        Public Sub mDependencies_Add(ByVal mDependency As _Dependencies)
            _dependencies.Add(mDependency)
        End Sub

        Property mIS() As List(Of String)
            Get
                Return _IS
            End Get
            Set(ByVal value As List(Of String))
                _IS = value
            End Set
        End Property

        Property mClass() As String
            Get
                Return _class
            End Get
            Set(ByVal value As String)
                _class = value
            End Set
        End Property

        Property mIncomp() As List(Of String)
            Get
                Return _incomp
            End Get
            Set(ByVal value As List(Of String))
                _incomp = value
            End Set
        End Property

        Property mSuccessors As List(Of String)
            Get
                Return _successors
            End Get
            Set(ByVal value As List(Of String))
                _successors = value
            End Set
        End Property

        ReadOnly Property mResources As List(Of _Resources)
            Get
                Return _resources
            End Get
        End Property

        Public Sub mResources_Add(ByVal mResource As _Resources)
            _resources.Add(mResource)
        End Sub

        ReadOnly Property mVariables As Dictionary(Of String, String)
            Get
                Return _variables
            End Get
        End Property

        Public Function mVariable_Get(ByVal mKey As String) As String
            Dim keys As Dictionary(Of String, String).KeyCollection = _variables.Keys
            For Each key As String In keys
                If mKey = key Then
                    Return _variables(key)
                End If
            Next
            Return Nothing
        End Function

        Public Function mVariables_Add(ByVal mKey As String, ByVal mValue As String) As Boolean
            Dim Result As Boolean = False

            If Not _variables.ContainsKey(mKey) Then
                _variables.Add(mKey, mValue)
                Result = True
            End If
            Return Result

        End Function

        Public Sub uproc()
            With Me
                .name = ""
                ._notsim = New List(Of _Dependencies)
                ._dependencies = New List(Of _Dependencies)
                ._IS = New List(Of String)
                ._class = ""
                ._incomp = New List(Of String)
                ._successors = New List(Of String)
                ._resources = New List(Of _Resources)
                ._variables = New Dictionary(Of String, String)
            End With
        End Sub

        Public Overrides Function toString() As String
            Dim it As Integer = 0
            Dim ret As String = "[uproc_name:" & name & ", class:" & _class

            ret &= ", incompatibilities:["
            it = _incomp.Count - 1
            For Each con1 As String In _incomp
                ret &= "[class:" & con1 & "]"
                If it > 0 Then ret &= ", "
                it -= 1
            Next

            ret &= "], successors:["
            it = _successors.Count - 1
            For Each con1 As String In _incomp
                ret &= "[uproc:" & con1 & "]"
                If it > 0 Then ret &= ", "
                it -= 1
            Next

            ret &= "], resources:["
            it = _resources.Count - 1
            For Each con1 As String In _incomp
                ret &= con1
                If it > 0 Then ret &= ", "
                it -= 1
            Next

            ret &= "], notsim:["
            it = _notsim.Count - 1
            For Each con1 As String In _incomp
                ret &= con1
                If it > 0 Then ret &= ", "
                it -= 1
            Next

            ret &= "], dependencies:["
            it = _dependencies.Count - 1
            For Each con1 As String In _incomp
                ret &= con1
                If it > 0 Then ret &= ", "
                it -= 1
            Next

            ret &= "], variables:["
            it = _variables.Count - 1
            For Each key As String In _variables.Keys
                ret &= "[variable:" & key & ", value:" & _variables.Item(key) & "]"
                If it > 0 Then ret &= ", "
                it -= 1
            Next
            ret &= "]]"

            Return ret
        End Function

        Public Function toHumanString(ByVal showIS As Boolean) As String
            Dim it As Integer = 0
            Dim ret As String = "UPROC Name: " & name & vbCrLf & _
                                "Class: " & _class & vbCrLf

            ret &= "Incompatibilities: " & vbCrLf
            For Each con1 As String In _incomp
                ret &= "  Class:" & con1 & vbCrLf
            Next

            ret &= "Successors:" & vbCrLf
            For Each con1 As String In _incomp
                ret &= "  Uproc :" & con1 & vbCrLf
            Next

            ret &= "Resources:" & vbCrLf
            For Each con1 As String In _incomp
                ret &= "  " & con1 & vbCrLf
            Next

            ret &= "NotSim:" & vbCrLf
            For Each con1 As String In _incomp
                ret &= "  " & con1 & vbCrLf
            Next

            ret &= "Dependencies:" & vbCrLf
            For Each con1 As String In _incomp
                ret &= "  " & con1 & vbCrLf
            Next

            ret &= "Variables:" & vbCrLf
            For Each key As String In _variables.Keys
                ret &= "  " & key & " = " & _variables.Item(key) & vbCrLf
            Next

            If showIS Then
                ret &= "Script:" & vbCrLf
                For Each line As String In _IS
                    ret &= "  " & line & vbCrLf
                Next
            End If

            ret &= vbCrLf

            Return ret
        End Function

        Public Overrides Function toJSONstring() As String
            Dim it As Integer = 0
            Dim ret As String = "{""uproc_name"":""" & name & """,""class"":""" & _class

            ret &= """,""incompatibilities"":["
            it = _incomp.Count - 1
            For Each con1 As String In _incomp
                ret &= "{""class"":""" & con1 & """}"
                If it > 0 Then ret &= ","
                it -= 1
            Next

            ret &= "],""successors"":["
            it = _successors.Count - 1
            For Each con1 As String In _incomp
                ret &= "{""uproc"":""" & con1 & """}"
                If it > 0 Then ret &= ","
                it -= 1
            Next

            ret &= "], ""resources"":["
            it = _resources.Count - 1
            For Each con1 As String In _incomp
                ret &= "{""" & con1 & """}"
                If it > 0 Then ret &= ","
                it -= 1
            Next

            ret &= "], ""notsim"":["
            it = _notsim.Count - 1
            For Each con1 As String In _incomp
                ret &= "{""" & con1 & """}"
                If it > 0 Then ret &= ","
                it -= 1
            Next

            ret &= "],""dependencies"":["
            it = _dependencies.Count - 1
            For Each con1 As String In _incomp
                ret &= "{""" & con1 & """}"
                If it > 0 Then ret &= ","
                it -= 1
            Next

            ret &= "], ""variables"":["
            it = _variables.Count - 1
            For Each key As String In _variables.Keys
                ret &= "{""variable"":""" & key & """, ""value"":""" & _variables.Item(key) & """}"
                If it > 0 Then ret &= ", "
                it -= 1
            Next
            ret &= "]}"

            Return ret
        End Function
    End Class
#End Region

#Region "_Session Class_DONE"
    Public Class _Session
        Inherits _baseObject
        Private _tree As List(Of _SessionTree)
        Private _label As String
        ReadOnly Property mTree As List(Of _SessionTree)
            Get
                Return _tree
            End Get
        End Property

        ReadOnly Property mLabel As String
            Get
                Return _label
            End Get
        End Property

        Public Function session(ByVal tmp As String, ByVal tree As List(Of _SessionTree), ByVal label As String)
            With Me
                .name = tmp
                ._tree = tree
                ._label = label
            End With
            Return Me
        End Function

        Public Overrides Function toString() As String
            Dim tmp As String
            Dim counter As Integer = _tree.Count

            tmp = "[name:" + Me.name + ", tree:["
            For Each item As _SessionTree In _tree
                counter -= 1
                tmp += item.ToString
                If counter > 0 Then
                    tmp += ", "
                End If
            Next

            tmp += "]]"

            Return tmp
        End Function

        Public Function toHumanString() As String
            Dim tmp As String
            Dim counter As Integer = _tree.Count

            tmp = "Session:" + Me.name + vbCrLf
            For Each item As _SessionTree In _tree
                counter -= 1
                tmp += item.ToHumanString
                If counter > 0 Then
                    tmp = tmp + vbCrLf
                End If
            Next

            tmp += vbCrLf

            Return tmp
        End Function


        Public Overrides Function toJSONstring() As String
            Dim tmp As String
            Dim counter As Integer = _tree.Count

            tmp = "{""name"":""" + Me.name + """, ""tree"":["
            For Each item As _SessionTree In _tree
                counter -= 1
                tmp += item.toJSONstring
                If counter > 0 Then
                    tmp += ", "
                End If
            Next

            tmp += "]}"

            Return tmp
        End Function
    End Class
#End Region

#Region "_SessionTree Class_DONE"
    Public Class _SessionTree
        Private _uproc As String
        Private _type As String
        Private _mu As String
        Private _parent As String
        Private _pid As Integer
        Private _connection_type As String

        ReadOnly Property mUproc As String
            Get
                Return _uproc
            End Get
        End Property

        ReadOnly Property mType As String
            Get
                Return _type
            End Get
        End Property

        ReadOnly Property mMU As String
            Get
                Return _mu
            End Get
        End Property

        ReadOnly Property mParent As String
            Get
                Return _parent
            End Get
        End Property

        ReadOnly Property mPid As Integer
            Get
                Return _pid
            End Get
        End Property

        ReadOnly Property mConnectionType As String
            Get
                Return _connection_type
            End Get
        End Property

        Public Function sessionTree(ByVal name As String, ByVal content As String, ByVal mu As String, ByVal parent As String, ByVal parentID As Integer, ByVal connection_type As String)
            With Me
                ._uproc = name
                ._type = content
                ._mu = mu
                ._parent = parentID
                ._pid = parentID
                ._connection_type = connection_type
            End With
            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return "[uproc:" + _uproc + ", type:" + _type + ", mu:" + _mu + ", parent:" + _parent + ", pid:" + CStr(_pid) + "]"
        End Function

        Public Function ToHumanString() As String
            Return " Uproc: " + _uproc + " - Type: " + _type + " - MU: " + _mu + " - Parent: " + _parent + " - pid: " + CStr(_pid)
        End Function

        Public Function toJSONstring() As String
            Return "{""uproc"":""" + _uproc + """, ""type"":""" + _type + """, ""mu"":""" + _mu + """, ""parent"":""" + _parent + """, ""pid"":""" + CStr(_pid) + """}"
        End Function
    End Class
#End Region

#Region "_Task Class_DONE"
    Public Class _Task
        Inherits _baseObject

        Private _session As String
        Private _uproc As String
        Private _launch As List(Of String)
        Private _is_active As Boolean
        Private _mu As String
        Private _template As Boolean
        Private _type As String
        Private _isUprocHeader As String
        Private _queue As String
        Private _rule As List(Of String)
        Private _optional As String
        Private _number_of_uprocs As Integer
        Private _variables As Dictionary(Of String, String)

        ReadOnly Property mSession As String
            Get
                Return _session
            End Get
        End Property

        ReadOnly Property mUproc As String
            Get
                Return _uproc
            End Get
        End Property

        ReadOnly Property mLaunch As List(Of String)
            Get
                Return _launch
            End Get
        End Property

        ReadOnly Property mIsActive As Boolean
            Get
                Return _is_active
            End Get
        End Property

        ReadOnly Property mMU As String
            Get
                Return _mu
            End Get
        End Property

        ReadOnly Property mTemplate As Boolean
            Get
                Return _template
            End Get
        End Property

        ReadOnly Property mType As String
            Get
                Return _type
            End Get
        End Property

        ReadOnly Property mIsUprocHeader As String
            Get
                Return _isUprocHeader
            End Get
        End Property

        ReadOnly Property mQueue As String
            Get
                Return _queue
            End Get
        End Property

        ReadOnly Property mRule As List(Of String)
            Get
                Return _rule
            End Get
        End Property

        ReadOnly Property mOptional As String
            Get
                Return _optional
            End Get
        End Property

        ReadOnly Property mNumberOfUprocs As Integer
            Get
                Return _number_of_uprocs
            End Get
        End Property

        ReadOnly Property mVariables As Dictionary(Of String, String)
            Get
                Return _variables
            End Get
        End Property

        Public Function mVariable_Get(ByVal mKey As String) As String
            Dim keys As Dictionary(Of String, String).KeyCollection = _variables.Keys
            For Each key As String In keys
                If mKey = key Then
                    Return _variables(key)
                End If
            Next
            Return Nothing
        End Function

        Public Function task(ByVal task_session As String, ByVal task_uproc As String, ByVal task_launch As List(Of String), ByVal task_active As String, ByVal task_mu As String, _
                        ByVal task_is_template As String, ByVal task_type As String, ByVal textContent As String, ByVal vars As Dictionary(Of String, String), ByVal queue As String, _
                        ByVal rule As List(Of String), ByVal opt As String, ByVal name As String)

            With Me
                ._session = task_session
                ._uproc = task_uproc
                ._launch = task_launch
                ._is_active = task_active
                ._mu = task_mu
                ._template = task_is_template
                ._type = task_type
                ._isUprocHeader = textContent
                ._variables = vars
                ._queue = queue
                ._rule = rule
                ._optional = opt
            End With

            If name = "" Then
                If task_session = "" Then
                    Me.name = "u!" + task_uproc + "!" + task_mu
                Else
                    Me.name = "s!" + task_session + "!" + task_mu
                End If
            Else
                Me.name = name
            End If
            Me._number_of_uprocs = vbNull

            Return Me
        End Function

        Public Overrides Function toString() As String
            Dim ret As String
            ret = "[name:" + name + ", session:" + _session + ", uproc:" + _uproc + ", mu:" + _mu + ", active:" + CStr(_is_active) + ", queue:" + _queue + _
                ", template:" + CStr(_template) + ", type:" + _type + ", optional:" + _optional + ", isUprocHeader:" + _isUprocHeader + ", number_of_uprocs:" + CStr(_number_of_uprocs)

            If _rule.Count = 0 Then
                ret = ret + ", rules:"
            Else
                ret = ret + ", rules:"
                For i As Integer = 0 To _rule.Count - 1
                    Dim rul As String = _rule.Item(i)
                    If i < _rule.Count - 1 Then
                        ret = ret + rul + ", "
                    Else
                        ret = ret + rul + "]"
                    End If
                Next
            End If

            Return ret
        End Function

        Public Function toHumanString() As String
            Dim ret As String
            ret = "TaskName: " + name + vbCrLf + _
                  "Session:" + _session + vbCrLf + _
                  "Uproc:" + _uproc + vbCrLf + _
                  "MU:" + _mu + vbCrLf + _
                  "Active:" + CStr(_is_active) + vbCrLf + _
                  "Queue:" + _queue + _
                  "Template:" + CStr(_template) + vbCrLf + _
                  "Type:" + _type + vbCrLf + _
                  "Optional:" + _optional + vbCrLf + _
                  "isUprocHeader:" + _isUprocHeader + vbCrLf + _
                  "Number_of_uprocs:" + CStr(_number_of_uprocs) + vbCrLf


            If _rule.Count = 0 Then
                ret = ret + "Rules: No rules"
            Else
                ret = ret + "Rules: " + vbCrLf
                Dim i As Integer = 1
                For Each rul As String In _rule
                    ret = ret + "   " + i.ToString + ": " + rul
                Next
            End If

            ret = ret + vbCrLf

            Return ret
        End Function

        Public Overrides Function toJSONstring() As String
            Dim ret As String

            ret = "{""name"":""" + name + """, ""session"":""" + _session + """, ""uproc"":""" + _uproc + """, ""mu"":""" + _mu + """, ""active"":""" + CStr(_is_active) + _
                  """, ""queue"":""" + _queue + """, ""template"":""" + CStr(_template) + """, ""type"":""" + _type + """, ""optional"":""" + _optional + _
                  """, ""isUprocHeader"":""" + _isUprocHeader + """, ""number_of_uprocs"":""" + CStr(_number_of_uprocs)


            If _rule.Count = 0 Then
                ret = ret + """, ""rules"":[]}"
            Else
                ret = ret + """, ""rules"":["""
                For i As Integer = 0 To _rule.Count - 1
                    Dim rul As String = _rule.Item(i)
                    If i < _rule.Count - 1 Then
                        ret += "" + rul + ""","""
                    Else
                        ret += "" + rul + """]}"
                    End If
                Next
            End If

            Return ret
        End Function

    End Class
#End Region

#Region "_Mu Class_DONE"
    Public Class _Mu
        Inherits _baseObject

        Private _node As String
        ReadOnly Property nodename '(ByVal mu_name As String)
            Get
                Return _node
            End Get
        End Property

        Public Function mu(ByVal name As String, ByVal node As String)
            With Me
                .name = name
                ._node = node
            End With
            Return Me
        End Function

        Public Overrides Function toString() As String
            Return "[mu:" + name + ", node:" + _node + "]"
        End Function

        Public Function toHumanString() As String
            Return "MU: " + name + "- Node: " + _node
        End Function

        Public Overrides Function toJSONstring() As String
            Return "{""mu"":""" + name + """, ""node"":""" + _node + """}"
        End Function
    End Class
#End Region

#Region "_Node Class"
    Public Class _Node
        Inherits _baseObject

        Private _uprocs As List(Of _Uproc)
        Private _sessions As List(Of _Session)
        Private _tasks As List(Of _Task)
        Private _mus As List(Of _Mu)
        Private _resources As List(Of _Resources)

        Property mUprocs As List(Of _Uproc)
            Get
                Return _uprocs
            End Get
            Set(ByVal value As List(Of _Uproc))
                _uprocs = value
            End Set
        End Property

        Property mSessions As List(Of _Session)
            Get
                Return _sessions
            End Get
            Set(ByVal value As List(Of _Session))
                _sessions = value
            End Set
        End Property

        Property mTasks As List(Of _Task)
            Get
                Return _tasks
            End Get
            Set(ByVal value As List(Of _Task))
                _tasks = value
            End Set
        End Property

        Property mMUs As List(Of _Mu)
            Get
                Return _mus
            End Get
            Set(ByVal value As List(Of _Mu))
                _mus = value
            End Set
        End Property

        Property mResources As List(Of _Resources)
            Get
                Return _resources
            End Get
            Set(ByVal value As List(Of _Resources))
                _resources = value
            End Set
        End Property

        Public Function node(ByVal nn As String, ByVal ss As List(Of _Session), ByVal mm As List(Of _Mu), ByVal tt As List(Of _Task), ByVal uu As List(Of _Uproc), ByVal rr As List(Of _Resources))
            With Me
                .name = nn
                ._uprocs = uu
                ._mus = mm
                ._sessions = ss
                ._tasks = tt
                ._resources = rr
            End With
            Return Me
        End Function

        Public Function toHumanString()
            Dim ret As String
            ret = "Node     : " + CStr(name) + vbCrLf + _
                  "Uprocs   : " + CStr(mUprocs.Count) + vbCrLf + _
                  "Sessions : " + CStr(mSessions.Count) + vbCrLf + _
                  "Tasks    : " + CStr(mTasks.Count) + vbCrLf + _
                  "Resources: " + CStr(mResources.Count) + vbCrLf
            Return ret
        End Function
    End Class
#End Region

#End Region

End Namespace