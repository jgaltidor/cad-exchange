Public Class SELine
    Public dbstart(5) As Double
    Public dbend(5) As Double
    Public type As String = "Line"

    Public Sub New(ByVal objLine2D As SolidEdgeFrameworkSupport.Line2d, ByVal objProfile As SolidEdgePart.Profile)
        Call objLine2D.GetStartPoint(dbstart(0), dbstart(1))
        Call objLine2D.GetEndPoint(dbend(0), dbend(1))
        Call objProfile.Convert2DCoordinate(dbstart(0), dbstart(1), dbstart(2), dbstart(3), dbstart(4))
        Call objProfile.Convert2DCoordinate(dbend(0), dbend(1), dbend(2), dbend(3), dbend(4))
    End Sub
End Class
