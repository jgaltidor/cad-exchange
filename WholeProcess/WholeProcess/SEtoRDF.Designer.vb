<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SEtoRDF
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
        Me.OpenSE1 = New System.Windows.Forms.Button
        Me.CreateRDF1 = New System.Windows.Forms.Button
        Me.SaveRDF = New System.Windows.Forms.Button
        Me.CheckOpen = New System.Windows.Forms.CheckBox
        Me.CheckCreate = New System.Windows.Forms.CheckBox
        Me.CheckSave = New System.Windows.Forms.CheckBox
        Me.OpenSE = New System.Windows.Forms.OpenFileDialog
        Me.SaveRDF1 = New System.Windows.Forms.SaveFileDialog
        Me.SuspendLayout()
        '
        'OpenSE1
        '
        Me.OpenSE1.Location = New System.Drawing.Point(14, 12)
        Me.OpenSE1.Name = "OpenSE1"
        Me.OpenSE1.Size = New System.Drawing.Size(200, 30)
        Me.OpenSE1.TabIndex = 0
        Me.OpenSE1.Text = "Open a SE Part File"
        Me.OpenSE1.UseVisualStyleBackColor = True
        '
        'CreateRDF1
        '
        Me.CreateRDF1.Location = New System.Drawing.Point(14, 63)
        Me.CreateRDF1.Name = "CreateRDF1"
        Me.CreateRDF1.Size = New System.Drawing.Size(200, 30)
        Me.CreateRDF1.TabIndex = 1
        Me.CreateRDF1.Text = "Create RDF1"
        Me.CreateRDF1.UseVisualStyleBackColor = True
        '
        'SaveRDF
        '
        Me.SaveRDF.Location = New System.Drawing.Point(14, 114)
        Me.SaveRDF.Name = "SaveRDF"
        Me.SaveRDF.Size = New System.Drawing.Size(200, 30)
        Me.SaveRDF.TabIndex = 2
        Me.SaveRDF.Text = "Save RDF1 File"
        Me.SaveRDF.UseVisualStyleBackColor = True
        '
        'CheckOpen
        '
        Me.CheckOpen.AutoSize = True
        Me.CheckOpen.Location = New System.Drawing.Point(227, 20)
        Me.CheckOpen.Name = "CheckOpen"
        Me.CheckOpen.Size = New System.Drawing.Size(15, 14)
        Me.CheckOpen.TabIndex = 3
        Me.CheckOpen.UseVisualStyleBackColor = True
        '
        'CheckCreate
        '
        Me.CheckCreate.AutoSize = True
        Me.CheckCreate.Location = New System.Drawing.Point(227, 71)
        Me.CheckCreate.Name = "CheckCreate"
        Me.CheckCreate.Size = New System.Drawing.Size(15, 14)
        Me.CheckCreate.TabIndex = 4
        Me.CheckCreate.UseVisualStyleBackColor = True
        '
        'CheckSave
        '
        Me.CheckSave.AutoSize = True
        Me.CheckSave.Location = New System.Drawing.Point(227, 122)
        Me.CheckSave.Name = "CheckSave"
        Me.CheckSave.Size = New System.Drawing.Size(15, 14)
        Me.CheckSave.TabIndex = 5
        Me.CheckSave.UseVisualStyleBackColor = True
        '
        'OpenSE
        '
        Me.OpenSE.FileName = "OpenFileDialog1"
        '
        'SEtoRDF
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(255, 158)
        Me.Controls.Add(Me.CheckSave)
        Me.Controls.Add(Me.CheckCreate)
        Me.Controls.Add(Me.CheckOpen)
        Me.Controls.Add(Me.SaveRDF)
        Me.Controls.Add(Me.CreateRDF1)
        Me.Controls.Add(Me.OpenSE1)
        Me.Name = "SEtoRDF"
        Me.Text = "SEtoRDF"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OpenSE1 As System.Windows.Forms.Button
    Friend WithEvents CreateRDF1 As System.Windows.Forms.Button
    Friend WithEvents SaveRDF As System.Windows.Forms.Button
    Friend WithEvents CheckOpen As System.Windows.Forms.CheckBox
    Friend WithEvents CheckCreate As System.Windows.Forms.CheckBox
    Friend WithEvents CheckSave As System.Windows.Forms.CheckBox
    Friend WithEvents OpenSE As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SaveRDF1 As System.Windows.Forms.SaveFileDialog
End Class
