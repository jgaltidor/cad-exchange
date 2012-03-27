Public Class AttrSurface
    Public Type As String
    Public Name As String
    Public Root(2) As Double
    Public Norm(2) As Double
    Public boolstatus As Boolean

    Public P1(2) As Double
    Public P2(2) As Double

    Public PlaneName As String

    Const x = 0.0000000001

    Public Sub SelcetSurface(ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object, ByRef SurfaceIndex As Integer)
        Dim i As Integer
        If P2(0) = 0 Then
            For i = 0 To 2
                If -x < Norm(i) And Norm(i) < x Then
                    Norm(i) = 0
                End If
                If -x < Root(i) And Root(i) < x Then
                    Root(i) = 0
                End If
            Next
            If (Norm(0) = 0) * (Norm(1) = 0) * ((Norm(2) = 1) + (Norm(2) = -1)) Then
                If (Root(0) = 0) * (Root(1) = 0) * (Root(2) = 0) Then
                    PlaneName = "Front Plane"
                Else
                    P1(2) = Root(2)
                    P2(2) = Root(2)
                    P1(0) = 1
                    P1(1) = -1
                    P2(0) = -1
                    P2(1) = 1
                    part.createplanefixed2(Root, P1, P2, False)

                    PlaneName = "Plane" & SurfaceIndex
                    SurfaceIndex += 1
                End If

                'boolstatus = docext.SelectByID2(PlaneName, "PLANE", 0, 0, 0, True, 1, Nothing, 0)
            ElseIf (Norm(0) = 0) * ((Norm(1) = 1) + (Norm(1) = -1)) * (Norm(2) = 0) Then
                If (Root(0) = 0) * (Root(1) = 0) * (Root(2) = 0) Then
                    PlaneName = "Top Plane"
                Else
                    P1(1) = Root(1)
                    P2(1) = Root(1)
                    P1(0) = 1
                    P1(2) = -1
                    P2(0) = -1
                    P2(2) = 1.5
                    part.createplanefixed2(Root, P1, P2, False)

                    PlaneName = "Plane" & SurfaceIndex
                    SurfaceIndex += 1
                End If
                'boolstatus = docext.SelectByID2(PlaneName, "PLANE", 0, 0, 0, True, 1, Nothing, 0)
            ElseIf ((Norm(0) = 1) + (Norm(0) = -1)) * (Norm(1) = 0) * (Norm(2) = 0) Then
                If (Root(0) = 0) * (Root(1) = 0) * (Root(2) = 0) Then
                    PlaneName = "Right Plane"
                Else
                    P1(0) = Root(0)
                    P2(0) = Root(0)
                    P1(1) = 1
                    P1(2) = -1
                    P2(1) = -1
                    P2(2) = 1
                    part.createplanefixed2(Root, P1, P2, False)

                    PlaneName = "Plane" & SurfaceIndex
                    SurfaceIndex += 1
                End If
                'boolstatus = docext.SelectByID2(PlaneName, "PLANE", 0, 0, 0, True, 1, Nothing, 0)

            ElseIf (Norm(0) <> 0) * (Norm(1) <> 0) * (Norm(2) = 0) Then
                P1(0) = Root(0) + 1
                P1(1) = Root(1) - Norm(0) / Norm(1)
                P1(2) = 1
                P2(0) = Root(0) - 1
                P2(1) = Root(1) + Norm(0) / Norm(1)
                P2(2) = 2
                part.createplanefixed2(Root, P1, P2, False)

                PlaneName = "Plane" & SurfaceIndex
                SurfaceIndex += 1
            ElseIf (Norm(0) <> 0) * (Norm(1) = 0) * (Norm(2) <> 0) Then
                P1(0) = Root(0) + 1
                P1(2) = Root(2) - Norm(0) / Norm(2)
                P1(1) = 1
                P2(0) = Root(0) - 1
                P2(2) = Root(2) + Norm(0) / Norm(2)
                P2(1) = 2
                part.createplanefixed2(Root, P1, P2, False)

                PlaneName = "Plane" & SurfaceIndex
                SurfaceIndex += 1
            ElseIf (Norm(0) = 0) * (Norm(1) <> 0) * (Norm(2) <> 0) Then
                P1(1) = Root(1) + 1
                P1(2) = Root(2) - Norm(1) / Norm(2)
                P1(0) = 1
                P2(1) = Root(1) - 1
                P2(2) = Root(2) + Norm(1) / Norm(2)
                P2(0) = 2

                part.createplanefixed2(Root, P1, P2, False)

                PlaneName = "Plane" & SurfaceIndex
                SurfaceIndex += 1
            Else

                P1(2) = Root(2) - Norm(1) / Norm(2)
                P2(2) = Root(2) - Norm(0) / Norm(2)
                P1(0) = Root(0)
                P1(1) = Root(1) + 1
                P2(0) = Root(0) + 1
                P2(1) = Root(1)


                'boolstatus = part.createpoint2(P1(0), P1(1), P1(2))
                'boolstatus = part.createpoint2(P2(0), P2(1), P2(2))
                'boolstatus = part.createline(P1, P2)
                'boolstatus = docext.SelectByID2("", "POINT", Root(0), Root(1), Root(2), True, 1, Nothing, 0)
                'boolstatus = docext.SelectByID2("", "POINT", P1(0), P1(1), P1(2), True, 16, Nothing, 0)
                'boolstatus = docext.SelectByID2("", "POINT", P2(0), P2(1), P2(2), True, 16, Nothing, 0)
                'part.CreatePlaneThru3Points2()

                part.createplanefixed2(Root, P1, P2, False)

                PlaneName = "Plane" & SurfaceIndex
                SurfaceIndex += 1

                'boolstatus = docext.SelectByID2("", "PLANE", Root(0), Root(1), Root(2), True, 1, Nothing, 0)
            End If
        Else
            part.createplanefixed2(Root, P1, P2, False)

            PlaneName = "Plane" & SurfaceIndex
            SurfaceIndex += 1
        End If
    End Sub
End Class
