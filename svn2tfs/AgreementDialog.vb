Public NotInheritable Class AgreementDialog

    Private Sub AgreementDialog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("License agreement of {0}", ApplicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.LabelProductName.Text = My.Application.Info.ProductName

        UpdateOkButtonState()

    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click, Button1.Click
        Me.Close()
    End Sub

    Private Sub StatusChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles licenseCheckBox.CheckedChanged, disclaimerCheckBox.CheckedChanged
        UpdateOkButtonState()
    End Sub

    Private Sub UpdateOkButtonState()
        OKButton.Enabled = licenseCheckBox.Checked And disclaimerCheckBox.Checked
    End Sub

End Class
