Imports <xmlns:kb="http://www.e-designcenter.info/kb#">

Public Class AttrSketch
    Public Sub DrawLine(ByRef record As XElement, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object)
        part.CreateLine2(record.@kb:Start3DX, record.@kb:Start3DY, record.@kb:Start3DZ, record.@kb:End3DX, record.@kb:End3DY, record.@kb:End3DZ)
    End Sub

    Public Sub DrawArc(ByRef record As XElement, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object)
        part.Create3PointArc(record.@kb:Start3DX, record.@kb:Start3DY, record.@kb:Start3DZ, _
                             record.@kb:End3DX, record.@kb:End3DY, record.@kb:End3DZ, _
                             record.@kb:Key3DX, record.@kb:Key3DY, record.@kb:Key3DZ)
    End Sub

    Public Sub DrawCircle(ByRef record As XElement, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object)
        part.Create3PointArc(record.@kb:Start3DX, record.@kb:Start3DY, record.@kb:Start3DZ, _
                             record.@kb:End3DX, record.@kb:End3DY, record.@kb:End3DZ, _
                             record.@kb:P13DX, record.@kb:P13DY, record.@kb:P13DZ)
        part.Create3PointArc(record.@kb:Start3DX, record.@kb:Start3DY, record.@kb:Start3DZ, _
                             record.@kb:End3DX, record.@kb:End3DY, record.@kb:End3DZ, _
                             record.@kb:P23DX, record.@kb:P23DY, record.@kb:P23DZ)
    End Sub
End Class
