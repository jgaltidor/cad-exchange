Public Class SECircle
    Public dbStart(5) As Double
    Public dbEnd(5) As Double
    Public dbCenter(5) As Double
    Public dbP1(5) As Double
    Public dbP2(5) As Double
    Public dbDiameter As Double
    Public dbRadius As Double

    Public Sub New(ByVal objCir2D As SolidEdgeFrameworkSupport.Circle2d, ByVal objProfile As SolidEdgePart.Profile)
        Call objCir2D.Getcenterpoint(dbCenter(0), dbCenter(1))
        dbDiameter = objCir2D.Diameter
        dbRadius = dbDiameter / 2
        dbStart(0) = dbCenter(0) - dbRadius
        dbStart(1) = dbCenter(1)
        dbEnd(0) = dbCenter(0) + dbRadius
        dbEnd(1) = dbCenter(1)
        dbP1(0) = dbCenter(0)
        dbP1(1) = dbCenter(1) - dbRadius
        dbP2(0) = dbCenter(0)
        dbP2(1) = dbCenter(1) + dbRadius

        Call objProfile.Convert2DCoordinate(dbStart(0), dbStart(1), dbStart(2), dbStart(3), dbStart(4))
        Call objProfile.Convert2DCoordinate(dbEnd(0), dbEnd(1), dbEnd(2), dbEnd(3), dbEnd(4))
        Call objProfile.Convert2DCoordinate(dbP1(0), dbP1(1), dbP1(2), dbP1(3), dbP1(4))
        Call objProfile.Convert2DCoordinate(dbP2(0), dbP2(1), dbP2(2), dbP2(3), dbP2(4))
        Call objProfile.Convert2DCoordinate(dbCenter(0), dbCenter(1), dbCenter(2), dbCenter(3), dbCenter(4))

    End Sub
End Class
