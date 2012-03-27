Public Class AttrLine
    Public Type As String
    Public Name As String
    Public Point1(2) As Double
    Public Point2(2) As Double
    Public boolstatus As Boolean


    Const x = 0.0000000001

    Public Sub SelectLine(ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object)
        Dim i As Integer

        For i = 0 To 2
            If -x < Point1(i) And Point1(i) < x Then
                Point1(i) = 0
            End If
            If -x < Point2(i) And Point2(i) < x Then
                Point2(i) = 0
            End If

        Next


    End Sub
End Class
