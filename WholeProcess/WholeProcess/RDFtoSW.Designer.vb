<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RDFtoSW
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
        Me.OpenRDF2 = New System.Windows.Forms.Button
        Me.CreateSW = New System.Windows.Forms.Button
        Me.SaveSW = New System.Windows.Forms.Button
        Me.RDF2Check = New System.Windows.Forms.CheckBox
        Me.SWCheck = New System.Windows.Forms.CheckBox
        Me.SWPartCheck = New System.Windows.Forms.CheckBox
        Me.OpenRDF = New System.Windows.Forms.OpenFileDialog
        Me.SaveSWPart = New System.Windows.Forms.SaveFileDialog
        Me.SuspendLayout()
        '
        'OpenRDF2
        '
        Me.OpenRDF2.Location = New System.Drawing.Point(14, 12)
        Me.OpenRDF2.Name = "OpenRDF2"
        Me.OpenRDF2.Size = New System.Drawing.Size(200, 30)
        Me.OpenRDF2.TabIndex = 0
        Me.OpenRDF2.Text = "Open RDF2"
        Me.OpenRDF2.UseVisualStyleBackColor = True
        '
        'CreateSW
        '
        Me.CreateSW.Location = New System.Drawing.Point(14, 63)
        Me.CreateSW.Name = "CreateSW"
        Me.CreateSW.Size = New System.Drawing.Size(200, 30)
        Me.CreateSW.TabIndex = 1
        Me.CreateSW.Text = "Create SW Part"
        Me.CreateSW.UseVisualStyleBackColor = True
        '
        'SaveSW
        '
        Me.SaveSW.Location = New System.Drawing.Point(14, 114)
        Me.SaveSW.Name = "SaveSW"
        Me.SaveSW.Size = New System.Drawing.Size(200, 30)
        Me.SaveSW.TabIndex = 2
        Me.SaveSW.Text = "Save SW Part File"
        Me.SaveSW.UseVisualStyleBackColor = True
        '
        'RDF2Check
        '
        Me.RDF2Check.AutoSize = True
        Me.RDF2Check.Location = New System.Drawing.Point(227, 20)
        Me.RDF2Check.Name = "RDF2Check"
        Me.RDF2Check.Size = New System.Drawing.Size(15, 14)
        Me.RDF2Check.TabIndex = 3
        Me.RDF2Check.UseVisualStyleBackColor = True
        '
        'SWCheck
        '
        Me.SWCheck.AutoSize = True
        Me.SWCheck.Location = New System.Drawing.Point(227, 71)
        Me.SWCheck.Name = "SWCheck"
        Me.SWCheck.Size = New System.Drawing.Size(15, 14)
        Me.SWCheck.TabIndex = 4
        Me.SWCheck.UseVisualStyleBackColor = True
        '
        'SWPartCheck
        '
        Me.SWPartCheck.AutoSize = True
        Me.SWPartCheck.Location = New System.Drawing.Point(227, 122)
        Me.SWPartCheck.Name = "SWPartCheck"
        Me.SWPartCheck.Size = New System.Drawing.Size(15, 14)
        Me.SWPartCheck.TabIndex = 5
        Me.SWPartCheck.UseVisualStyleBackColor = True
        '
        'OpenRDF
        '
        Me.OpenRDF.FileName = "OpenFileDialog1"
        '
        'RDFtoSW
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(255, 158)
        Me.Controls.Add(Me.SWPartCheck)
        Me.Controls.Add(Me.SWCheck)
        Me.Controls.Add(Me.RDF2Check)
        Me.Controls.Add(Me.SaveSW)
        Me.Controls.Add(Me.CreateSW)
        Me.Controls.Add(Me.OpenRDF2)
        Me.Name = "RDFtoSW"
        Me.Text = "RDFtoSW"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OpenRDF2 As System.Windows.Forms.Button
    Friend WithEvents CreateSW As System.Windows.Forms.Button
    Friend WithEvents SaveSW As System.Windows.Forms.Button
    Friend WithEvents RDF2Check As System.Windows.Forms.CheckBox
    Friend WithEvents SWCheck As System.Windows.Forms.CheckBox
    Friend WithEvents SWPartCheck As System.Windows.Forms.CheckBox
    Friend WithEvents OpenRDF As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SaveSWPart As System.Windows.Forms.SaveFileDialog
End Class
