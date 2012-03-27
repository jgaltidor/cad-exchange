Public Class SEArc
    Public dbStart(5) As Double
    Public dbEnd(5) As Double
    Public dbCenter(5) As Double
    Public dbKey(5) As Double
    Public dbDiameter As Double
    Public dbRadius As Double

    Public Sub New(ByVal objArc2D As SolidEdgeFrameworkSupport.Arc2d, ByVal objProfile As SolidEdgePart.Profile)
        Call objArc2D.GetCenterPoint(dbCenter(0), dbCenter(1))
        dbRadius = objArc2D.Radius
        dbDiameter = dbRadius * 2
        Call objArc2D.GetStartPoint(dbStart(0), dbStart(1))
        Call objArc2D.GetEndPoint(dbEnd(0), dbEnd(1))
        Call objArc2D.GetKeyPoint(3, dbKey(0), dbKey(1), dbKey(2), 32, 0)
        Call objProfile.Convert2DCoordinate(dbStart(0), dbStart(1), dbStart(2), dbStart(3), dbStart(4))
        Call objProfile.Convert2DCoordinate(dbEnd(0), dbEnd(1), dbEnd(2), dbEnd(3), dbEnd(4))
        Call objProfile.Convert2DCoordinate(dbCenter(0), dbCenter(1), dbCenter(2), dbCenter(3), dbCenter(4))
        Call objProfile.Convert2DCoordinate(dbKey(0), dbKey(1), dbKey(2), dbKey(3), dbKey(4))
    End Sub
End Class
