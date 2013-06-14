<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim MySettings11 As svn2tfs.My.MySettings = New svn2tfs.My.MySettings()
        Dim MySettings12 As svn2tfs.My.MySettings = New svn2tfs.My.MySettings()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainWindow))
        Dim MySettings10 As svn2tfs.My.MySettings = New svn2tfs.My.MySettings()
        Me.userMapOpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.standardImportRadioButton = New System.Windows.Forms.RadioButton()
        Me.mainTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.consoleTabControl = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.logTextBox = New System.Windows.Forms.TextBox()
        Me.WarningsTabPage = New System.Windows.Forms.TabPage()
        Me.errorsListView = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.buttonsPanel = New System.Windows.Forms.Panel()
        Me.bugCommand = New System.Windows.Forms.Button()
        Me.aboutButton = New System.Windows.Forms.Button()
        Me.importButton = New System.Windows.Forms.Button()
        Me.abortButton = New System.Windows.Forms.Button()
        Me.upperTabControl = New System.Windows.Forms.TabControl()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.svnDirectoryTextBox = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.tfsDirectoryTextBox = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.svnPasswordTextBox = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.svnUserNameTextBox = New System.Windows.Forms.TextBox()
        Me.openUserMappingFileNameButton = New System.Windows.Forms.Button()
        Me.svnUrlTextBox = New System.Windows.Forms.TextBox()
        Me.tfsUrlTextBox = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.testTfsButton = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tfsProjectTextBox = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.helpUserMappingButton = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.fromRevisionTextBox = New System.Windows.Forms.TextBox()
        Me.tfsCollectionTextBox = New System.Windows.Forms.TextBox()
        Me.toRevisionTextBox = New System.Windows.Forms.TextBox()
        Me.userMapFileNameTextBox = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.testSvnButton = New System.Windows.Forms.Button()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.customTemporaryFolderBrowseButton = New System.Windows.Forms.Button()
        Me.customTemporaryFolderTextBox = New System.Windows.Forms.TextBox()
        Me.customTemporaryFolderCheckBox = New System.Windows.Forms.CheckBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.overrideFirstRevisionRadioButton = New System.Windows.Forms.RadioButton()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.cleanUpTFSCheckBox = New System.Windows.Forms.CheckBox()
        Me.customTemporaryFolderOpenDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.mainTableLayoutPanel.SuspendLayout()
        Me.consoleTabControl.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.WarningsTabPage.SuspendLayout()
        Me.buttonsPanel.SuspendLayout()
        Me.upperTabControl.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'userMapOpenFileDialog
        '
        Me.userMapOpenFileDialog.Filter = "User mapping (*.xml)|*.xml|All files (*.*)|*.*"
        Me.userMapOpenFileDialog.Title = "Open an XML user mapping file..."
        '
        'standardImportRadioButton
        '
        Me.standardImportRadioButton.AutoSize = True
        Me.standardImportRadioButton.Checked = True
        Me.standardImportRadioButton.Location = New System.Drawing.Point(3, 3)
        Me.standardImportRadioButton.Name = "standardImportRadioButton"
        Me.standardImportRadioButton.Size = New System.Drawing.Size(103, 17)
        Me.standardImportRadioButton.TabIndex = 30
        Me.standardImportRadioButton.TabStop = True
        Me.standardImportRadioButton.Text = "Replay all action"
        Me.standardImportRadioButton.UseVisualStyleBackColor = True
        '
        'mainTableLayoutPanel
        '
        Me.mainTableLayoutPanel.ColumnCount = 1
        Me.mainTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.mainTableLayoutPanel.Controls.Add(Me.consoleTabControl, 0, 2)
        Me.mainTableLayoutPanel.Controls.Add(Me.buttonsPanel, 0, 1)
        Me.mainTableLayoutPanel.Controls.Add(Me.upperTabControl, 0, 0)
        Me.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mainTableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.mainTableLayoutPanel.Name = "mainTableLayoutPanel"
        Me.mainTableLayoutPanel.Padding = New System.Windows.Forms.Padding(10)
        Me.mainTableLayoutPanel.RowCount = 3
        Me.mainTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 337.0!))
        Me.mainTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.mainTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.mainTableLayoutPanel.Size = New System.Drawing.Size(561, 597)
        Me.mainTableLayoutPanel.TabIndex = 17
        '
        'consoleTabControl
        '
        Me.consoleTabControl.Controls.Add(Me.TabPage1)
        Me.consoleTabControl.Controls.Add(Me.WarningsTabPage)
        Me.consoleTabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.consoleTabControl.Location = New System.Drawing.Point(13, 396)
        Me.consoleTabControl.Multiline = True
        Me.consoleTabControl.Name = "consoleTabControl"
        Me.consoleTabControl.SelectedIndex = 0
        Me.consoleTabControl.Size = New System.Drawing.Size(535, 188)
        Me.consoleTabControl.TabIndex = 102
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.logTextBox)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(527, 162)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Output"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'logTextBox
        '
        Me.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.logTextBox.Location = New System.Drawing.Point(3, 3)
        Me.logTextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.logTextBox.Multiline = True
        Me.logTextBox.Name = "logTextBox"
        Me.logTextBox.ReadOnly = True
        Me.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.logTextBox.Size = New System.Drawing.Size(521, 156)
        Me.logTextBox.TabIndex = 17
        '
        'WarningsTabPage
        '
        Me.WarningsTabPage.Controls.Add(Me.errorsListView)
        Me.WarningsTabPage.Location = New System.Drawing.Point(4, 22)
        Me.WarningsTabPage.Name = "WarningsTabPage"
        Me.WarningsTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.WarningsTabPage.Size = New System.Drawing.Size(527, 162)
        Me.WarningsTabPage.TabIndex = 1
        Me.WarningsTabPage.Text = "WarningsTabPage"
        Me.WarningsTabPage.UseVisualStyleBackColor = True
        '
        'errorsListView
        '
        Me.errorsListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.errorsListView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.errorsListView.FullRowSelect = True
        Me.errorsListView.GridLines = True
        Me.errorsListView.Location = New System.Drawing.Point(3, 3)
        Me.errorsListView.Name = "errorsListView"
        Me.errorsListView.Size = New System.Drawing.Size(521, 199)
        Me.errorsListView.TabIndex = 0
        Me.errorsListView.UseCompatibleStateImageBehavior = False
        Me.errorsListView.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Warning description"
        Me.ColumnHeader1.Width = 468
        '
        'buttonsPanel
        '
        Me.buttonsPanel.Controls.Add(Me.bugCommand)
        Me.buttonsPanel.Controls.Add(Me.aboutButton)
        Me.buttonsPanel.Controls.Add(Me.importButton)
        Me.buttonsPanel.Controls.Add(Me.abortButton)
        Me.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.buttonsPanel.Location = New System.Drawing.Point(13, 350)
        Me.buttonsPanel.Name = "buttonsPanel"
        Me.buttonsPanel.Size = New System.Drawing.Size(535, 40)
        Me.buttonsPanel.TabIndex = 14
        '
        'bugCommand
        '
        Me.bugCommand.Location = New System.Drawing.Point(116, 8)
        Me.bugCommand.Name = "bugCommand"
        Me.bugCommand.Size = New System.Drawing.Size(106, 26)
        Me.bugCommand.TabIndex = 104
        Me.bugCommand.Text = "Fill a bug"
        Me.bugCommand.UseVisualStyleBackColor = True
        '
        'aboutButton
        '
        Me.aboutButton.Location = New System.Drawing.Point(4, 8)
        Me.aboutButton.Name = "aboutButton"
        Me.aboutButton.Size = New System.Drawing.Size(106, 26)
        Me.aboutButton.TabIndex = 103
        Me.aboutButton.Text = "About"
        Me.aboutButton.UseVisualStyleBackColor = True
        '
        'importButton
        '
        Me.importButton.Location = New System.Drawing.Point(309, 8)
        Me.importButton.Name = "importButton"
        Me.importButton.Size = New System.Drawing.Size(106, 26)
        Me.importButton.TabIndex = 100
        Me.importButton.Text = "Import"
        Me.importButton.UseVisualStyleBackColor = True
        '
        'abortButton
        '
        Me.abortButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.abortButton.Location = New System.Drawing.Point(421, 8)
        Me.abortButton.Name = "abortButton"
        Me.abortButton.Size = New System.Drawing.Size(106, 26)
        Me.abortButton.TabIndex = 101
        Me.abortButton.Text = "Cancel"
        Me.abortButton.UseVisualStyleBackColor = True
        '
        'upperTabControl
        '
        Me.upperTabControl.Controls.Add(Me.TabPage3)
        Me.upperTabControl.Controls.Add(Me.TabPage4)
        Me.upperTabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.upperTabControl.Location = New System.Drawing.Point(13, 13)
        Me.upperTabControl.Name = "upperTabControl"
        Me.upperTabControl.SelectedIndex = 0
        Me.upperTabControl.Size = New System.Drawing.Size(535, 331)
        Me.upperTabControl.TabIndex = 19
        '
        'TabPage3
        '
        Me.TabPage3.AutoScroll = True
        Me.TabPage3.Controls.Add(Me.Label20)
        Me.TabPage3.Controls.Add(Me.svnDirectoryTextBox)
        Me.TabPage3.Controls.Add(Me.Label19)
        Me.TabPage3.Controls.Add(Me.tfsDirectoryTextBox)
        Me.TabPage3.Controls.Add(Me.Label13)
        Me.TabPage3.Controls.Add(Me.Label4)
        Me.TabPage3.Controls.Add(Me.Label10)
        Me.TabPage3.Controls.Add(Me.svnPasswordTextBox)
        Me.TabPage3.Controls.Add(Me.Label8)
        Me.TabPage3.Controls.Add(Me.svnUserNameTextBox)
        Me.TabPage3.Controls.Add(Me.openUserMappingFileNameButton)
        Me.TabPage3.Controls.Add(Me.svnUrlTextBox)
        Me.TabPage3.Controls.Add(Me.tfsUrlTextBox)
        Me.TabPage3.Controls.Add(Me.Label6)
        Me.TabPage3.Controls.Add(Me.testTfsButton)
        Me.TabPage3.Controls.Add(Me.Label3)
        Me.TabPage3.Controls.Add(Me.tfsProjectTextBox)
        Me.TabPage3.Controls.Add(Me.Label9)
        Me.TabPage3.Controls.Add(Me.helpUserMappingButton)
        Me.TabPage3.Controls.Add(Me.Label2)
        Me.TabPage3.Controls.Add(Me.Label7)
        Me.TabPage3.Controls.Add(Me.Label1)
        Me.TabPage3.Controls.Add(Me.Label11)
        Me.TabPage3.Controls.Add(Me.fromRevisionTextBox)
        Me.TabPage3.Controls.Add(Me.tfsCollectionTextBox)
        Me.TabPage3.Controls.Add(Me.toRevisionTextBox)
        Me.TabPage3.Controls.Add(Me.userMapFileNameTextBox)
        Me.TabPage3.Controls.Add(Me.Label5)
        Me.TabPage3.Controls.Add(Me.testSvnButton)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(527, 305)
        Me.TabPage3.TabIndex = 0
        Me.TabPage3.Text = "Basics"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(268, 24)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(33, 13)
        Me.Label20.TabIndex = 26
        Me.Label20.Text = "Path:"
        '
        'svnDirectoryTextBox
        '
        MySettings11.customTemporaryFolder = ""
        MySettings11.hasAcceptedAgreement = False
        MySettings11.lastDeleteTfsProject = False
        MySettings11.lastFromRevision = "1"
        MySettings11.lastOverrideFirstAction = False
        MySettings11.lastReplayAllActions = True
        MySettings11.lastSvnUrl = ""
        MySettings11.lastSvnUserName = ""
        MySettings11.lastTfsCollection = ""
        MySettings11.lastTfsDirectory = ""
        MySettings11.lastTfsProject = ""
        MySettings11.lastTfsURL = ""
        MySettings11.lastTfsUserName = ""
        MySettings11.lastToRevision = "HEAD"
        MySettings11.lastUserMappingFileName = ""
        MySettings11.lstTfsDomain = ""
        MySettings11.SettingsKey = ""
        MySettings11.userCustomTemporaryFolder = False
        Me.svnDirectoryTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", MySettings11, "lastSvnUrl", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.svnDirectoryTextBox.Location = New System.Drawing.Point(336, 20)
        Me.svnDirectoryTextBox.Name = "svnDirectoryTextBox"
        Me.svnDirectoryTextBox.Size = New System.Drawing.Size(161, 21)
        Me.svnDirectoryTextBox.TabIndex = 25
        Me.svnDirectoryTextBox.Text = MySettings11.lastSvnUrl
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(267, 222)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(41, 13)
        Me.Label19.TabIndex = 23
        Me.Label19.Text = "Folder:"
        '
        'tfsDirectoryTextBox
        '
        Me.tfsDirectoryTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.svn2tfs.My.MySettings.Default, "lastTfsDirectory", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.tfsDirectoryTextBox.Location = New System.Drawing.Point(336, 218)
        Me.tfsDirectoryTextBox.Name = "tfsDirectoryTextBox"
        Me.tfsDirectoryTextBox.Size = New System.Drawing.Size(165, 21)
        Me.tfsDirectoryTextBox.TabIndex = 24
        Me.tfsDirectoryTextBox.Text = Global.svn2tfs.My.MySettings.Default.lastTfsDirectory
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(6, 204)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(136, 13)
        Me.Label13.TabIndex = 21
        Me.Label13.Text = "Team Foundation 2010"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(18, 222)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(30, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "URL:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(6, 3)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(90, 13)
        Me.Label10.TabIndex = 20
        Me.Label10.Text = "SubVersion 6.x"
        '
        'svnPasswordTextBox
        '
        Me.svnPasswordTextBox.Location = New System.Drawing.Point(337, 47)
        Me.svnPasswordTextBox.Name = "svnPasswordTextBox"
        Me.svnPasswordTextBox.Size = New System.Drawing.Size(160, 21)
        Me.svnPasswordTextBox.TabIndex = 3
        Me.svnPasswordTextBox.UseSystemPasswordChar = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(267, 248)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(45, 13)
        Me.Label8.TabIndex = 14
        Me.Label8.Text = "Project:"
        '
        'svnUserNameTextBox
        '
        MySettings12.customTemporaryFolder = ""
        MySettings12.hasAcceptedAgreement = False
        MySettings12.lastDeleteTfsProject = False
        MySettings12.lastFromRevision = "1"
        MySettings12.lastOverrideFirstAction = False
        MySettings12.lastReplayAllActions = True
        MySettings12.lastSvnUrl = ""
        MySettings12.lastSvnUserName = ""
        MySettings12.lastTfsCollection = ""
        MySettings12.lastTfsDirectory = ""
        MySettings12.lastTfsProject = ""
        MySettings12.lastTfsURL = ""
        MySettings12.lastTfsUserName = ""
        MySettings12.lastToRevision = "HEAD"
        MySettings12.lastUserMappingFileName = ""
        MySettings12.lstTfsDomain = ""
        MySettings12.SettingsKey = ""
        MySettings12.userCustomTemporaryFolder = False
        Me.svnUserNameTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", MySettings12, "lastSvnUserName", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.svnUserNameTextBox.Location = New System.Drawing.Point(100, 47)
        Me.svnUserNameTextBox.Name = "svnUserNameTextBox"
        Me.svnUserNameTextBox.Size = New System.Drawing.Size(160, 21)
        Me.svnUserNameTextBox.TabIndex = 2
        Me.svnUserNameTextBox.Text = MySettings12.lastSvnUserName
        '
        'openUserMappingFileNameButton
        '
        Me.openUserMappingFileNameButton.Location = New System.Drawing.Point(155, 175)
        Me.openUserMappingFileNameButton.Name = "openUserMappingFileNameButton"
        Me.openUserMappingFileNameButton.Size = New System.Drawing.Size(106, 26)
        Me.openUserMappingFileNameButton.TabIndex = 9
        Me.openUserMappingFileNameButton.Text = "Open..."
        Me.openUserMappingFileNameButton.UseVisualStyleBackColor = True
        '
        'svnUrlTextBox
        '
        Me.svnUrlTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", MySettings10, "lastSvnUrl", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.svnUrlTextBox.Location = New System.Drawing.Point(100, 21)
        Me.svnUrlTextBox.Name = "svnUrlTextBox"
        Me.svnUrlTextBox.Size = New System.Drawing.Size(159, 21)
        Me.svnUrlTextBox.TabIndex = 1
        Me.svnUrlTextBox.Text = MySettings10.lastSvnUrl
        '
        'tfsUrlTextBox
        '
        Me.tfsUrlTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", MySettings10, "lastTfsURL", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.tfsUrlTextBox.Location = New System.Drawing.Point(104, 219)
        Me.tfsUrlTextBox.Name = "tfsUrlTextBox"
        Me.tfsUrlTextBox.Size = New System.Drawing.Size(155, 21)
        Me.tfsUrlTextBox.TabIndex = 10
        Me.tfsUrlTextBox.Text = MySettings10.lastTfsURL
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(14, 77)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(75, 13)
        Me.Label6.TabIndex = 8
        Me.Label6.Text = "From revision:"
        '
        'testTfsButton
        '
        Me.testTfsButton.Location = New System.Drawing.Point(43, 272)
        Me.testTfsButton.Name = "testTfsButton"
        Me.testTfsButton.Size = New System.Drawing.Size(106, 26)
        Me.testTfsButton.TabIndex = 13
        Me.testTfsButton.Text = "Validate TFS"
        Me.testTfsButton.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(268, 50)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(57, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Password:"
        '
        'tfsProjectTextBox
        '
        Me.tfsProjectTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", MySettings10, "lastTfsProject", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.tfsProjectTextBox.Location = New System.Drawing.Point(336, 245)
        Me.tfsProjectTextBox.Name = "tfsProjectTextBox"
        Me.tfsProjectTextBox.Size = New System.Drawing.Size(165, 21)
        Me.tfsProjectTextBox.TabIndex = 12
        Me.tfsProjectTextBox.Text = MySettings10.lastTfsProject
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(268, 77)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(63, 13)
        Me.Label9.TabIndex = 9
        Me.Label9.Text = "To revision:"
        '
        'helpUserMappingButton
        '
        Me.helpUserMappingButton.Location = New System.Drawing.Point(43, 175)
        Me.helpUserMappingButton.Name = "helpUserMappingButton"
        Me.helpUserMappingButton.Size = New System.Drawing.Size(106, 26)
        Me.helpUserMappingButton.TabIndex = 8
        Me.helpUserMappingButton.Text = "Help..."
        Me.helpUserMappingButton.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(14, 50)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(59, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Username:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(18, 248)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(57, 13)
        Me.Label7.TabIndex = 11
        Me.Label7.Text = "Collection:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(30, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "URL:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(6, 130)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(85, 13)
        Me.Label11.TabIndex = 21
        Me.Label11.Text = "User mapping"
        '
        'fromRevisionTextBox
        '
        Me.fromRevisionTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", MySettings10, "lastFromRevision", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.fromRevisionTextBox.Location = New System.Drawing.Point(100, 74)
        Me.fromRevisionTextBox.Name = "fromRevisionTextBox"
        Me.fromRevisionTextBox.Size = New System.Drawing.Size(160, 21)
        Me.fromRevisionTextBox.TabIndex = 4
        Me.fromRevisionTextBox.Text = MySettings10.lastFromRevision
        '
        'tfsCollectionTextBox
        '
        Me.tfsCollectionTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", MySettings10, "lastTfsCollection", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.tfsCollectionTextBox.Location = New System.Drawing.Point(104, 245)
        Me.tfsCollectionTextBox.Name = "tfsCollectionTextBox"
        Me.tfsCollectionTextBox.Size = New System.Drawing.Size(155, 21)
        Me.tfsCollectionTextBox.TabIndex = 11
        Me.tfsCollectionTextBox.Text = MySettings10.lastTfsCollection
        '
        'toRevisionTextBox
        '
        Me.toRevisionTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", MySettings10, "lastToRevision", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.toRevisionTextBox.Location = New System.Drawing.Point(337, 74)
        Me.toRevisionTextBox.Name = "toRevisionTextBox"
        Me.toRevisionTextBox.Size = New System.Drawing.Size(160, 21)
        Me.toRevisionTextBox.TabIndex = 5
        Me.toRevisionTextBox.Text = MySettings10.lastToRevision
        '
        'userMapFileNameTextBox
        '
        Me.userMapFileNameTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", MySettings10, "lastUserMappingFileName", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.userMapFileNameTextBox.Location = New System.Drawing.Point(100, 148)
        Me.userMapFileNameTextBox.Name = "userMapFileNameTextBox"
        Me.userMapFileNameTextBox.Size = New System.Drawing.Size(397, 21)
        Me.userMapFileNameTextBox.TabIndex = 7
        Me.userMapFileNameTextBox.Text = MySettings10.lastUserMappingFileName
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(14, 151)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(73, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "User map file:"
        '
        'testSvnButton
        '
        Me.testSvnButton.Location = New System.Drawing.Point(43, 101)
        Me.testSvnButton.Name = "testSvnButton"
        Me.testSvnButton.Size = New System.Drawing.Size(106, 26)
        Me.testSvnButton.TabIndex = 6
        Me.testSvnButton.Text = "Validate SVN"
        Me.testSvnButton.UseVisualStyleBackColor = True
        '
        'TabPage4
        '
        Me.TabPage4.AutoScroll = True
        Me.TabPage4.Controls.Add(Me.customTemporaryFolderBrowseButton)
        Me.TabPage4.Controls.Add(Me.customTemporaryFolderTextBox)
        Me.TabPage4.Controls.Add(Me.customTemporaryFolderCheckBox)
        Me.TabPage4.Controls.Add(Me.Label12)
        Me.TabPage4.Controls.Add(Me.Label16)
        Me.TabPage4.Controls.Add(Me.Label15)
        Me.TabPage4.Controls.Add(Me.Panel1)
        Me.TabPage4.Controls.Add(Me.Label14)
        Me.TabPage4.Controls.Add(Me.cleanUpTFSCheckBox)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(527, 305)
        Me.TabPage4.TabIndex = 1
        Me.TabPage4.Text = "Under the hood"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'customTemporaryFolderBrowseButton
        '
        Me.customTemporaryFolderBrowseButton.Location = New System.Drawing.Point(398, 265)
        Me.customTemporaryFolderBrowseButton.Name = "customTemporaryFolderBrowseButton"
        Me.customTemporaryFolderBrowseButton.Size = New System.Drawing.Size(106, 26)
        Me.customTemporaryFolderBrowseButton.TabIndex = 35
        Me.customTemporaryFolderBrowseButton.Text = "Browse..."
        Me.customTemporaryFolderBrowseButton.UseVisualStyleBackColor = True
        '
        'customTemporaryFolderTextBox
        '
        Me.customTemporaryFolderTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.svn2tfs.My.MySettings.Default, "customTemporaryFolder", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.customTemporaryFolderTextBox.Location = New System.Drawing.Point(201, 269)
        Me.customTemporaryFolderTextBox.Name = "customTemporaryFolderTextBox"
        Me.customTemporaryFolderTextBox.Size = New System.Drawing.Size(191, 21)
        Me.customTemporaryFolderTextBox.TabIndex = 34
        Me.customTemporaryFolderTextBox.Text = Global.svn2tfs.My.MySettings.Default.customTemporaryFolder
        '
        'customTemporaryFolderCheckBox
        '
        Me.customTemporaryFolderCheckBox.AutoSize = True
        Me.customTemporaryFolderCheckBox.Checked = Global.svn2tfs.My.MySettings.Default.userCustomTemporaryFolder
        Me.customTemporaryFolderCheckBox.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.svn2tfs.My.MySettings.Default, "userCustomTemporaryFolder", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.customTemporaryFolderCheckBox.Location = New System.Drawing.Point(17, 271)
        Me.customTemporaryFolderCheckBox.Name = "customTemporaryFolderCheckBox"
        Me.customTemporaryFolderCheckBox.Size = New System.Drawing.Size(178, 17)
        Me.customTemporaryFolderCheckBox.TabIndex = 33
        Me.customTemporaryFolderCheckBox.Text = "Use a custom temporary folder:"
        Me.customTemporaryFolderCheckBox.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(5, 254)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(106, 13)
        Me.Label12.TabIndex = 25
        Me.Label12.Text = "Temporary folder"
        '
        'Label16
        '
        Me.Label16.AccessibleName = ""
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(45, 206)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(413, 39)
        Me.Label16.TabIndex = 24
        Me.Label16.Text = "Deletes all files and directories in the destination before the import." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "A check-" & _
            "in will be commited to apply these changes, so actual data can be recovered" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "at " & _
            "the end of the import process."
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(6, 167)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(113, 13)
        Me.Label15.TabIndex = 23
        Me.Label15.Text = "Pre-import actions"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Label18)
        Me.Panel1.Controls.Add(Me.Label17)
        Me.Panel1.Controls.Add(Me.overrideFirstRevisionRadioButton)
        Me.Panel1.Controls.Add(Me.standardImportRadioButton)
        Me.Panel1.Location = New System.Drawing.Point(14, 24)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(490, 140)
        Me.Panel1.TabIndex = 22
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Enabled = False
        Me.Label18.Location = New System.Drawing.Point(31, 94)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(367, 39)
        Me.Label18.TabIndex = 26
        Me.Label18.Text = "The starting state of the source is reproduced to the target." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "This is accomplish" & _
            "ed substituting the actions of the starting revision with an" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "'add everything re" & _
            "cursively' action."
        '
        'Label17
        '
        Me.Label17.AccessibleName = ""
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(32, 23)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(402, 39)
        Me.Label17.TabIndex = 25
        Me.Label17.Text = resources.GetString("Label17.Text")
        '
        'overrideFirstRevisionRadioButton
        '
        Me.overrideFirstRevisionRadioButton.AutoSize = True
        MySettings10.customTemporaryFolder = ""
        MySettings10.hasAcceptedAgreement = False
        MySettings10.lastDeleteTfsProject = False
        MySettings10.lastFromRevision = "1"
        MySettings10.lastOverrideFirstAction = False
        MySettings10.lastReplayAllActions = True
        MySettings10.lastSvnUrl = ""
        MySettings10.lastSvnUserName = ""
        MySettings10.lastTfsCollection = ""
        MySettings10.lastTfsDirectory = ""
        MySettings10.lastTfsProject = ""
        MySettings10.lastTfsURL = ""
        MySettings10.lastTfsUserName = ""
        MySettings10.lastToRevision = "HEAD"
        MySettings10.lastUserMappingFileName = ""
        MySettings10.lstTfsDomain = ""
        MySettings10.SettingsKey = ""
        MySettings10.userCustomTemporaryFolder = False
        Me.overrideFirstRevisionRadioButton.Checked = MySettings10.lastOverrideFirstAction
        Me.overrideFirstRevisionRadioButton.DataBindings.Add(New System.Windows.Forms.Binding("Checked", MySettings10, "lastOverrideFirstAction", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.overrideFirstRevisionRadioButton.Enabled = False
        Me.overrideFirstRevisionRadioButton.Location = New System.Drawing.Point(3, 74)
        Me.overrideFirstRevisionRadioButton.Name = "overrideFirstRevisionRadioButton"
        Me.overrideFirstRevisionRadioButton.Size = New System.Drawing.Size(159, 17)
        Me.overrideFirstRevisionRadioButton.TabIndex = 31
        Me.overrideFirstRevisionRadioButton.Text = "Reproduce state and replay"
        Me.overrideFirstRevisionRadioButton.UseVisualStyleBackColor = True
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(6, 3)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(94, 13)
        Me.Label14.TabIndex = 21
        Me.Label14.Text = "Import method"
        '
        'cleanUpTFSCheckBox
        '
        Me.cleanUpTFSCheckBox.AutoSize = True
        Me.cleanUpTFSCheckBox.Checked = MySettings10.lastDeleteTfsProject
        Me.cleanUpTFSCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cleanUpTFSCheckBox.DataBindings.Add(New System.Windows.Forms.Binding("Checked", MySettings10, "lastDeleteTfsProject", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.cleanUpTFSCheckBox.Location = New System.Drawing.Point(17, 186)
        Me.cleanUpTFSCheckBox.Name = "cleanUpTFSCheckBox"
        Me.cleanUpTFSCheckBox.Size = New System.Drawing.Size(336, 17)
        Me.cleanUpTFSCheckBox.TabIndex = 32
        Me.cleanUpTFSCheckBox.Text = "Delete all Team Foundation Server project content before import"
        Me.cleanUpTFSCheckBox.UseVisualStyleBackColor = True
        '
        'MainWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(561, 597)
        Me.Controls.Add(Me.mainTableLayoutPanel)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(577, 578)
        Me.Name = "MainWindow"
        Me.Text = "title"
        Me.mainTableLayoutPanel.ResumeLayout(False)
        Me.consoleTabControl.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.WarningsTabPage.ResumeLayout(False)
        Me.buttonsPanel.ResumeLayout(False)
        Me.upperTabControl.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage4.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents userMapOpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents toRevisionTextBox As System.Windows.Forms.TextBox
    Friend WithEvents fromRevisionTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents testSvnButton As System.Windows.Forms.Button
    Friend WithEvents svnPasswordTextBox As System.Windows.Forms.TextBox
    Friend WithEvents svnUserNameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents svnUrlTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents testTfsButton As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents tfsProjectTextBox As System.Windows.Forms.TextBox
    Friend WithEvents tfsCollectionTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents tfsUrlTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents buttonsPanel As System.Windows.Forms.Panel
    Friend WithEvents aboutButton As System.Windows.Forms.Button
    Friend WithEvents importButton As System.Windows.Forms.Button
    Friend WithEvents abortButton As System.Windows.Forms.Button
    Friend WithEvents overrideFirstRevisionRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents standardImportRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents cleanUpTFSCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents helpUserMappingButton As System.Windows.Forms.Button
    Friend WithEvents openUserMappingFileNameButton As System.Windows.Forms.Button
    Friend WithEvents userMapFileNameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents consoleTabControl As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents logTextBox As System.Windows.Forms.TextBox
    Friend WithEvents WarningsTabPage As System.Windows.Forms.TabPage
    Friend WithEvents errorsListView As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents mainTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents upperTabControl As System.Windows.Forms.TabControl
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents bugCommand As System.Windows.Forms.Button
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents customTemporaryFolderTextBox As System.Windows.Forms.TextBox
    Friend WithEvents customTemporaryFolderCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents customTemporaryFolderOpenDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents customTemporaryFolderBrowseButton As System.Windows.Forms.Button
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents tfsDirectoryTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents svnDirectoryTextBox As System.Windows.Forms.TextBox

End Class
