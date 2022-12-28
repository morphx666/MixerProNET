Imports System.xml
Imports System.Reflection

Public Class ClassXMLExplorer
    Public Function GetXML(ByVal obj As Object) As String
        Dim xd As XmlDocument = New XmlDocument
        Dim xn As XmlNode = xd.AppendChild(xd.CreateNode(XmlNodeType.Element, obj.GetType.FullName, ""))

        p2XML(obj, xn)

        Return xd.InnerXml
    End Function

    Private Sub p2XML(ByRef obj As Object, ByRef pNode As XmlNode)
        Dim objType As Type = obj.GetType
        Dim Properties() As PropertyInfo = objType.GetProperties(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.DeclaredOnly)
        Dim custAttr As Type = GetType(DontSaveInXML)
        Dim xd As XmlDocument = pNode.OwnerDocument

        For Each p As PropertyInfo In Properties
            If Not System.Attribute.IsDefined(objType.GetMember(p.Name)(0), custAttr) Then
                If p.CanRead And p.CanWrite And p.Name <> "Parent" And p.Name <> "Container" Then
                    Dim vo As Object = p.GetValue(obj, Nothing)

                    If Not vo Is Nothing Then
                        Dim v As Object
                        v = vo

                        Dim x As XmlNode = pNode.AppendChild(xd.CreateNode(XmlNodeType.Element, "property", ""))
                        x.AppendChild(xd.CreateNode(XmlNodeType.Element, "name", "")).InnerText = p.Name
                        x.AppendChild(xd.CreateNode(XmlNodeType.Element, "type", "")).InnerText = vo.GetType.FullName.ToString

                        'If p.PropertyType Is GetType(Object) Then
                        addPropVal(p, v, x, xd)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub addPropVal(ByVal p As PropertyInfo, ByVal v As Object, ByVal x As XmlNode, ByVal xd As XmlDocument)
        If p.PropertyType.IsClass Then
            If v.GetType.IsArray Then
                For Each sv As Object In v
                    addPropVal(p, sv, x, xd)
                Next
            Else
                x.AppendChild(xd.CreateNode(XmlNodeType.Element, "value", "")).InnerXml = GetXML(v)
            End If
            'p2XML(v, pNode.AppendChild(xd.CreateNode(XmlNodeType.Element, "object", "")))
        Else
            x.AppendChild(xd.CreateNode(XmlNodeType.Element, "value", "")).InnerText = v.ToString()
        End If
    End Sub
End Class

<AttributeUsage(AttributeTargets.Property)> _
Public Class DontSaveInXML
    Inherits System.Attribute
End Class
