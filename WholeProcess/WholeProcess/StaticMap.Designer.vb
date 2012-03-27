<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StaticMap
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
        Me.S_OpenLib1 = New System.Windows.Forms.Button
        Me.S_OpenLib2 = New System.Windows.Forms.Button
        Me.S_OpenRDF1 = New System.Windows.Forms.Button
        Me.S_CreateRDF2 = New System.Windows.Forms.Button
        Me.Lib2Check = New System.Windows.Forms.CheckBox
        Me.RDF1Check = New System.Windows.Forms.CheckBox
        Me.RDF2Check = New System.Windows.Forms.CheckBox
        Me.Lib1Check = New System.Windows.Forms.CheckBox
        Me.OpenLib1 = New System.Windows.Forms.OpenFileDialog
        Me.OpenLib2 = New System.Windows.Forms.OpenFileDialog
        Me.OpenRDF1 = New System.Windows.Forms.OpenFileDialog
        Me.SaveRDF2 = New System.Windows.Forms.SaveFileDialog
        Me.SuspendLayout()
        '
        'S_OpenLib1
        '
        Me.S_OpenLib1.Location = New System.Drawing.Point(14, 12)
        Me.S_OpenLib1.Name = "S_OpenLib1"
        Me.S_OpenLib1.Size = New System.Drawing.Size(80, 30)
        Me.S_OpenLib1.TabIndex = 0
        Me.S_OpenLib1.Text = "Open Lib1"
        Me.S_OpenLib1.UseVisualStyleBackColor = True
        '
        'S_OpenLib2
        '
        Me.S_OpenLib2.Location = New System.Drawing.Point(132, 12)
        Me.S_OpenLib2.Name = "S_OpenLib2"
        Me.S_OpenLib2.Size = New System.Drawing.Size(80, 30)
        Me.S_OpenLib2.TabIndex = 1
        Me.S_OpenLib2.Text = "Open Lib2"
        Me.S_OpenLib2.UseVisualStyleBackColor = True
        '
        'S_OpenRDF1
        '
        Me.S_OpenRDF1.Location = New System.Drawing.Point(14, 63)
        Me.S_OpenRDF1.Name = "S_OpenRDF1"
        Me.S_OpenRDF1.Size = New System.Drawing.Size(200, 30)
        Me.S_OpenRDF1.TabIndex = 2
        Me.S_OpenRDF1.Text = "Open RDF1"
        Me.S_OpenRDF1.UseVisualStyleBackColor = True
        '
        'S_CreateRDF2
        '
        Me.S_CreateRDF2.Location = New System.Drawing.Point(14, 114)
        Me.S_CreateRDF2.Name = "S_CreateRDF2"
        Me.S_CreateRDF2.Size = New System.Drawing.Size(200, 30)
        Me.S_CreateRDF2.TabIndex = 3
        Me.S_CreateRDF2.Text = "Create RDF2"
        Me.S_CreateRDF2.UseVisualStyleBackColor = True
        '
        'Lib2Check
        '
        Me.Lib2Check.AutoSize = True
        Me.Lib2Check.Location = New System.Drawing.Point(227, 20)
        Me.Lib2Check.Name = "Lib2Check"
        Me.Lib2Check.Size = New System.Drawing.Size(15, 14)
        Me.Lib2Check.TabIndex = 4
        Me.Lib2Check.UseVisualStyleBackColor = True
        '
        'RDF1Check
        '
        Me.RDF1Check.AutoSize = True
        Me.RDF1Check.Location = New System.Drawing.Point(227, 71)
        Me.RDF1Check.Name = "RDF1Check"
        Me.RDF1Check.Size = New System.Drawing.Size(15, 14)
        Me.RDF1Check.TabIndex = 5
        Me.RDF1Check.UseVisualStyleBackColor = True
        '
        'RDF2Check
        '
        Me.RDF2Check.AutoSize = True
        Me.RDF2Check.Location = New System.Drawing.Point(227, 122)
        Me.RDF2Check.Name = "RDF2Check"
        Me.RDF2Check.Size = New System.Drawing.Size(15, 14)
        Me.RDF2Check.TabIndex = 6
        Me.RDF2Check.UseVisualStyleBackColor = True
        '
        'Lib1Check
        '
        Me.Lib1Check.AutoSize = True
        Me.Lib1Check.Location = New System.Drawing.Point(107, 21)
        Me.Lib1Check.Name = "Lib1Check"
        Me.Lib1Check.Size = New System.Drawing.Size(15, 14)
        Me.Lib1Check.TabIndex = 7
        Me.Lib1Check.UseVisualStyleBackColor = True
        '
        'OpenLib1
        '
        Me.OpenLib1.FileName = "OpenFileDialog1"
        '
        'OpenLib2
        '
        Me.OpenLib2.FileName = "OpenFileDialog1"
        '
        'OpenRDF1
        '
        Me.OpenRDF1.FileName = "OpenFileDialog1"
        '
        'StaticMap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(255, 158)
        Me.Controls.Add(Me.Lib1Check)
        Me.Controls.Add(Me.RDF2Check)
        Me.Controls.Add(Me.RDF1Check)
        Me.Controls.Add(Me.Lib2Check)
        Me.Controls.Add(Me.S_CreateRDF2)
        Me.Controls.Add(Me.S_OpenRDF1)
        Me.Controls.Add(Me.S_OpenLib2)
        Me.Controls.Add(Me.S_OpenLib1)
        Me.Name = "StaticMap"
        Me.Text = "StaticMap"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents S_OpenLib1 As System.Windows.Forms.Button
    Friend WithEvents S_OpenLib2 As System.Windows.Forms.Button
    Friend WithEvents S_OpenRDF1 As System.Windows.Forms.Button
    Friend WithEvents S_CreateRDF2 As System.Windows.Forms.Button
    Friend WithEvents Lib2Check As System.Windows.Forms.CheckBox
    Friend WithEvents RDF1Check As System.Windows.Forms.CheckBox
    Friend WithEvents RDF2Check As System.Windows.Forms.CheckBox
    Friend WithEvents Lib1Check As System.Windows.Forms.CheckBox
    Friend WithEvents OpenLib1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents OpenLib2 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents OpenRDF1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SaveRDF2 As System.Windows.Forms.SaveFileDialog
End Class
