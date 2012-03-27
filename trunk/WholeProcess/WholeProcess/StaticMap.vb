Imports System
Imports System.IO


Public Class StaticMap

    Dim Lib1Name As String
    Dim Lib2Name As String
    Dim RDF1Name As String

    Private Sub S_OpenLib1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles S_OpenLib1.Click
        If OpenLib1.ShowDialog() = DialogResult.OK Then
            Dim myStreamReader As New StreamReader(OpenLib1.FileName)
            Lib1Name = OpenLib1.FileName
            Dim FExt As String = OpenLib1.DefaultExt
            myStreamReader.Close()
            Lib1Check.Checked = True
        End If

    End Sub

    Private Sub S_OpenLib2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles S_OpenLib2.Click
        If OpenLib2.ShowDialog() = DialogResult.OK Then
            Dim myStreamReader As New StreamReader(OpenLib2.FileName)
            lib2Name = OpenLib2.FileName
            Dim FExt As String = OpenLib2.DefaultExt
            myStreamReader.Close()
            Lib2Check.Checked = True
        End If

    End Sub

    Private Sub S_OpenRDF1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles S_OpenRDF1.Click
        If OpenRDF1.ShowDialog() = DialogResult.OK Then
            Dim myStreamReader As New StreamReader(OpenRDF1.FileName)
            RDF1Name = OpenRDF1.FileName
            Dim FExt As String = OpenRDF1.DefaultExt
            myStreamReader.Close()
            RDF1Check.Checked = True
        End If
    End Sub

    Private Sub S_CreateRDF2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles S_CreateRDF2.Click

    End Sub
End Class