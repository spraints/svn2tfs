Imports System.Windows.Forms
Imports System.Text

Public Class TextBoxWriter
    Inherits System.IO.TextWriter

    Private control As TextBoxBase
    Private bufferStringBuilder As StringBuilder

    Public Sub New(ByVal control As TextBox)
        Me.control = control
        AddHandler control.HandleCreated, _
           New EventHandler(AddressOf OnHandleCreated)
    End Sub

    Public Overrides Sub Write(ByVal ch As Char)
        Write(ch.ToString())
    End Sub

    Public Overrides Sub Write(ByVal s As String)
        If (control.IsHandleCreated) Then
            AppendText(s)
        Else
            BufferText(s)
        End If
    End Sub

    Public Overrides Sub WriteLine(ByVal s As String)
        Write(s + Environment.NewLine)
    End Sub

    Private Sub BufferText(ByVal s As String)
        If (bufferStringBuilder Is Nothing) Then
            bufferStringBuilder = New StringBuilder()
        End If
        bufferStringBuilder.Append(s)
    End Sub

    Private Sub AppendText(ByVal s As String)
        If (bufferStringBuilder Is Nothing = False) Then
            control.AppendText(bufferStringBuilder.ToString())
            bufferStringBuilder = Nothing
        End If
        control.AppendText(s)
    End Sub

    Private Sub OnHandleCreated(ByVal sender As Object, _
       ByVal e As EventArgs)
        If (bufferStringBuilder Is Nothing = False) Then
            control.AppendText(bufferStringBuilder.ToString())
            bufferStringBuilder = Nothing
        End If
    End Sub

    Public Overrides ReadOnly Property Encoding() As System.Text.Encoding
        Get
            Return Encoding.Default
        End Get
    End Property

End Class