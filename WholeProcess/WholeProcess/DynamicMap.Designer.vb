<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DynamicMap
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
        Me.D_OpenLib2 = New System.Windows.Forms.Button
        Me.D_OpenRDF1 = New System.Windows.Forms.Button
        Me.D_CreateRDF2 = New System.Windows.Forms.Button
        Me.Lib2Check = New System.Windows.Forms.CheckBox
        Me.RDF1Check = New System.Windows.Forms.CheckBox
        Me.OpenLib2 = New System.Windows.Forms.OpenFileDialog
        Me.OpenRDF1 = New System.Windows.Forms.OpenFileDialog
        Me.SaveRDF2 = New System.Windows.Forms.SaveFileDialog
        Me.RDF1 = New System.Windows.Forms.GroupBox
        Me.RDF1_Redundant = New System.Windows.Forms.TextBox
        Me.RDF1_Common = New System.Windows.Forms.TextBox
        Me.RDF1_Feature = New System.Windows.Forms.TextBox
        Me.Lib2 = New System.Windows.Forms.GroupBox
        Me.Lib2_Missing = New System.Windows.Forms.TextBox
        Me.Lib2_Common = New System.Windows.Forms.TextBox
        Me.Lib2_Feature = New System.Windows.Forms.TextBox
        Me.Result = New System.Windows.Forms.TextBox
        Me.Best_Result = New System.Windows.Forms.TextBox
        Me.Match = New System.Windows.Forms.Label
        Me.Best_Match = New System.Windows.Forms.Label
        Me.RDF1_Name = New System.Windows.Forms.TextBox
        Me.Lib2_Name = New System.Windows.Forms.TextBox
        Me.RDF1.SuspendLayout()
        Me.Lib2.SuspendLayout()
        Me.SuspendLayout()
        '
        'D_OpenLib2
        '
        Me.D_OpenLib2.Location = New System.Drawing.Point(318, 12)
        Me.D_OpenLib2.Name = "D_OpenLib2"
        Me.D_OpenLib2.Size = New System.Drawing.Size(100, 30)
        Me.D_OpenLib2.TabIndex = 0
        Me.D_OpenLib2.Text = "Open Lib2"
        Me.D_OpenLib2.UseVisualStyleBackColor = True
        '
        'D_OpenRDF1
        '
        Me.D_OpenRDF1.Location = New System.Drawing.Point(14, 12)
        Me.D_OpenRDF1.Name = "D_OpenRDF1"
        Me.D_OpenRDF1.Size = New System.Drawing.Size(100, 30)
        Me.D_OpenRDF1.TabIndex = 1
        Me.D_OpenRDF1.Text = "Open RDF1"
        Me.D_OpenRDF1.UseVisualStyleBackColor = True
        '
        'D_CreateRDF2
        '
        Me.D_CreateRDF2.Location = New System.Drawing.Point(502, 406)
        Me.D_CreateRDF2.Name = "D_CreateRDF2"
        Me.D_CreateRDF2.Size = New System.Drawing.Size(100, 30)
        Me.D_CreateRDF2.TabIndex = 2
        Me.D_CreateRDF2.Text = "Mapping"
        Me.D_CreateRDF2.UseVisualStyleBackColor = True
        '
        'Lib2Check
        '
        Me.Lib2Check.AutoSize = True
        Me.Lib2Check.Location = New System.Drawing.Point(431, 20)
        Me.Lib2Check.Name = "Lib2Check"
        Me.Lib2Check.Size = New System.Drawing.Size(15, 14)
        Me.Lib2Check.TabIndex = 3
        Me.Lib2Check.UseVisualStyleBackColor = True
        '
        'RDF1Check
        '
        Me.RDF1Check.AutoSize = True
        Me.RDF1Check.Location = New System.Drawing.Point(127, 20)
        Me.RDF1Check.Name = "RDF1Check"
        Me.RDF1Check.Size = New System.Drawing.Size(15, 14)
        Me.RDF1Check.TabIndex = 4
        Me.RDF1Check.UseVisualStyleBackColor = True
        '
        'OpenLib2
        '
        Me.OpenLib2.FileName = "OpenFileDialog1"
        '
        'OpenRDF1
        '
        Me.OpenRDF1.FileName = "OpenFileDialog1"
        '
        'RDF1
        '
        Me.RDF1.Controls.Add(Me.RDF1_Redundant)
        Me.RDF1.Controls.Add(Me.RDF1_Common)
        Me.RDF1.Controls.Add(Me.RDF1_Feature)
        Me.RDF1.Location = New System.Drawing.Point(14, 48)
        Me.RDF1.Name = "RDF1"
        Me.RDF1.Size = New System.Drawing.Size(270, 310)
        Me.RDF1.TabIndex = 10
        Me.RDF1.TabStop = False
        Me.RDF1.Text = "RDF1_Group"
        Me.RDF1.UseCompatibleTextRendering = True
        '
        'RDF1_Redundant
        '
        Me.RDF1_Redundant.Location = New System.Drawing.Point(12, 176)
        Me.RDF1_Redundant.Multiline = True
        Me.RDF1_Redundant.Name = "RDF1_Redundant"
        Me.RDF1_Redundant.Size = New System.Drawing.Size(247, 128)
        Me.RDF1_Redundant.TabIndex = 2
        Me.RDF1_Redundant.Text = "Redundant Attributes"
        '
        'RDF1_Common
        '
        Me.RDF1_Common.Location = New System.Drawing.Point(12, 45)
        Me.RDF1_Common.Multiline = True
        Me.RDF1_Common.Name = "RDF1_Common"
        Me.RDF1_Common.Size = New System.Drawing.Size(247, 125)
        Me.RDF1_Common.TabIndex = 1
        Me.RDF1_Common.Text = "Common Attributes"
        '
        'RDF1_Feature
        '
        Me.RDF1_Feature.Location = New System.Drawing.Point(12, 19)
        Me.RDF1_Feature.Name = "RDF1_Feature"
        Me.RDF1_Feature.Size = New System.Drawing.Size(247, 20)
        Me.RDF1_Feature.TabIndex = 0
        Me.RDF1_Feature.Text = "RDF1_Feature"
        '
        'Lib2
        '
        Me.Lib2.Controls.Add(Me.Lib2_Missing)
        Me.Lib2.Controls.Add(Me.Lib2_Common)
        Me.Lib2.Controls.Add(Me.Lib2_Feature)
        Me.Lib2.Location = New System.Drawing.Point(318, 48)
        Me.Lib2.Name = "Lib2"
        Me.Lib2.Size = New System.Drawing.Size(270, 310)
        Me.Lib2.TabIndex = 11
        Me.Lib2.TabStop = False
        Me.Lib2.Text = "Lib2_Group"
        '
        'Lib2_Missing
        '
        Me.Lib2_Missing.Location = New System.Drawing.Point(13, 176)
        Me.Lib2_Missing.Multiline = True
        Me.Lib2_Missing.Name = "Lib2_Missing"
        Me.Lib2_Missing.Size = New System.Drawing.Size(247, 128)
        Me.Lib2_Missing.TabIndex = 3
        Me.Lib2_Missing.Text = "Missing Attributes"
        '
        'Lib2_Common
        '
        Me.Lib2_Common.Location = New System.Drawing.Point(13, 45)
        Me.Lib2_Common.Multiline = True
        Me.Lib2_Common.Name = "Lib2_Common"
        Me.Lib2_Common.Size = New System.Drawing.Size(247, 125)
        Me.Lib2_Common.TabIndex = 2
        Me.Lib2_Common.Text = "Common Attributes"
        '
        'Lib2_Feature
        '
        Me.Lib2_Feature.Location = New System.Drawing.Point(13, 19)
        Me.Lib2_Feature.Name = "Lib2_Feature"
        Me.Lib2_Feature.Size = New System.Drawing.Size(247, 20)
        Me.Lib2_Feature.TabIndex = 1
        Me.Lib2_Feature.Text = "Lib2_Feature"
        '
        'Result
        '
        Me.Result.Location = New System.Drawing.Point(15, 377)
        Me.Result.Multiline = True
        Me.Result.Name = "Result"
        Me.Result.Size = New System.Drawing.Size(236, 83)
        Me.Result.TabIndex = 12
        Me.Result.Text = "Result"
        '
        'Best_Result
        '
        Me.Best_Result.Location = New System.Drawing.Point(260, 377)
        Me.Best_Result.Multiline = True
        Me.Best_Result.Name = "Best_Result"
        Me.Best_Result.Size = New System.Drawing.Size(236, 83)
        Me.Best_Result.TabIndex = 13
        Me.Best_Result.Text = "Best Result"
        '
        'Match
        '
        Me.Match.AutoSize = True
        Me.Match.Location = New System.Drawing.Point(12, 361)
        Me.Match.Name = "Match"
        Me.Match.Size = New System.Drawing.Size(65, 13)
        Me.Match.TabIndex = 14
        Me.Match.Text = "Match result"
        '
        'Best_Match
        '
        Me.Best_Match.AutoSize = True
        Me.Best_Match.Location = New System.Drawing.Point(257, 361)
        Me.Best_Match.Name = "Best_Match"
        Me.Best_Match.Size = New System.Drawing.Size(88, 13)
        Me.Best_Match.TabIndex = 15
        Me.Best_Match.Text = "Best match result"
        '
        'RDF1_Name
        '
        Me.RDF1_Name.Location = New System.Drawing.Point(156, 22)
        Me.RDF1_Name.Name = "RDF1_Name"
        Me.RDF1_Name.Size = New System.Drawing.Size(117, 20)
        Me.RDF1_Name.TabIndex = 16
        '
        'Lib2_Name
        '
        Me.Lib2_Name.Location = New System.Drawing.Point(461, 22)
        Me.Lib2_Name.Name = "Lib2_Name"
        Me.Lib2_Name.Size = New System.Drawing.Size(117, 20)
        Me.Lib2_Name.TabIndex = 17
        '
        'DynamicMap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(616, 472)
        Me.Controls.Add(Me.Lib2_Name)
        Me.Controls.Add(Me.RDF1_Name)
        Me.Controls.Add(Me.Best_Match)
        Me.Controls.Add(Me.Match)
        Me.Controls.Add(Me.Best_Result)
        Me.Controls.Add(Me.Result)
        Me.Controls.Add(Me.Lib2)
        Me.Controls.Add(Me.RDF1)
        Me.Controls.Add(Me.RDF1Check)
        Me.Controls.Add(Me.Lib2Check)
        Me.Controls.Add(Me.D_CreateRDF2)
        Me.Controls.Add(Me.D_OpenRDF1)
        Me.Controls.Add(Me.D_OpenLib2)
        Me.Name = "DynamicMap"
        Me.Text = "DynamicMapping"
        Me.RDF1.ResumeLayout(False)
        Me.RDF1.PerformLayout()
        Me.Lib2.ResumeLayout(False)
        Me.Lib2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents D_OpenLib2 As System.Windows.Forms.Button
    Friend WithEvents D_OpenRDF1 As System.Windows.Forms.Button
    Friend WithEvents D_CreateRDF2 As System.Windows.Forms.Button
    Friend WithEvents Lib2Check As System.Windows.Forms.CheckBox
    Friend WithEvents RDF1Check As System.Windows.Forms.CheckBox
    Friend WithEvents OpenLib2 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents OpenRDF1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SaveRDF2 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents RDF1 As System.Windows.Forms.GroupBox
    Friend WithEvents Lib2 As System.Windows.Forms.GroupBox
    Friend WithEvents RDF1_Common As System.Windows.Forms.TextBox
    Friend WithEvents RDF1_Feature As System.Windows.Forms.TextBox
    Friend WithEvents Lib2_Feature As System.Windows.Forms.TextBox
    Friend WithEvents RDF1_Redundant As System.Windows.Forms.TextBox
    Friend WithEvents Lib2_Common As System.Windows.Forms.TextBox
    Friend WithEvents Lib2_Missing As System.Windows.Forms.TextBox
    Friend WithEvents Result As System.Windows.Forms.TextBox
    Friend WithEvents Best_Result As System.Windows.Forms.TextBox
    Friend WithEvents Match As System.Windows.Forms.Label
    Friend WithEvents Best_Match As System.Windows.Forms.Label
    Friend WithEvents RDF1_Name As System.Windows.Forms.TextBox
    Friend WithEvents Lib2_Name As System.Windows.Forms.TextBox
End Class
