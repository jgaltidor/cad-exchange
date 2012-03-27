'This is to extract all feature information of a SolidEdge model file, and record them as a RDF file.
'The version of SolidEdge is V20.
'The basic thinking behind the code is using API count the number of features first, then goto each feature to
'Get its type to decide which attributes are used, then get the detail parameters of each attributes.

Imports System.IO
Imports System
Imports System.Linq
Imports System.Xml.Linq


Public Class SEtoRDF
    Dim SEFileName As String
    Dim objApp As SolidEdgeFramework.Application
    Dim objDocs As Object
    Dim objDoc As SolidEdgePart.PartDocument
    Dim rfile As XDocument


    Public Sub OpenSE1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenSE1.Click

        If OpenSE.ShowDialog() = DialogResult.OK Then
            Dim myStreamReader As New StreamReader(OpenSE.FileName)
            SEFileName = OpenSE.FileName
            Dim FExt As String = OpenSE.DefaultExt
            myStreamReader.Close()

            'CREATE SolidEdge Application
            Try
                objApp = GetObject(, "SolidEdge.Application")
                objApp.Visible = True
            Catch ex As Exception
                objApp = CreateObject("SolidEdge.Application")
                objApp.Visible = True
            End Try
            objDocs = objApp.Documents
            objDoc = objDocs.Open(SEFileName)

            CheckOpen.Checked = True
        End If
    End Sub

    Public Sub CreateRDF1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CreateRDF1.Click



        Dim objFeatures As Object
        Dim objFeature As Object
        Dim objFeatureName As String = ""
        Dim objFeatureExtentType As Integer
        Dim objFeatureExtentSide As Integer
        Dim objProfile As Object
        Dim objPlane As Object
        Dim objPlanes As Object

        'Dim objFromSurface As Object
        'Dim objFromSurfaceRoot(0 To 2) As Double
        'Dim objFromSurfaceNorm(0 To 2) As Double
        'Dim objToSurface As Object
        'Dim objToSurfaceRoot(0 To 2) As Double
        'Dim objToSurfaceNorm(0 To 2) As Double

        Dim objParameter As Double

        Dim objExtrudedProtrusion As SolidEdgePart.ExtrudedProtrusion
        Dim objExtrudedCutout As SolidEdgePart.ExtrudedCutout
        Dim objRound As SolidEdgePart.Round
        Dim objMirrorCopy As SolidEdgePart.MirrorCopy

        Dim i As Integer
        Dim j As Integer
        'Dim k As Integer

        Dim objLines2D As Object
        'Dim objLine As SELine
        Dim objCirs As Object
        'Dim objCir As SECircle
        Dim objArcs2D As Object
        'Dim objArc As SEArc

        Dim FeatureIndex As Integer = 1
        Dim PointIndex As Integer = 1
        Dim LineIndex As Integer = 1
        Dim SurfaceIndex As Integer = 1
        Dim BodyIndex As Integer = 1
        Dim SketchIndex As Integer = 1
        Dim ParaIndex As Integer = 1
        Dim BoolIndex As Integer = 1


        Dim objInputFeature(8) As Object
        'All features in ObjDoc
        objFeatures = objApp.ActiveDocument.Models(1).Features

        'Title of the RDF file
        rfile = New XDocument(New XDeclaration("1.0", "utf-8", ""))
        Dim rdf As XNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim rdfs As XNamespace = "http://www.w3.org/2000/01/rdf-schema#"
        Dim nRDF As New XElement(rdf + "RDF")
        rFile.Add(nRDF)
        Dim xa1 As New XAttribute(XNamespace.Xmlns + "rdf", rdf.NamespaceName)
        Dim xa2 As New XAttribute(XNamespace.Xmlns + "kb", kb.NamespaceName)
        Dim xa3 As New XAttribute(XNamespace.Xmlns + "rdfs", rdfs.NamespaceName)
        nRDF.Add(xa1)
        nRDF.Add(xa2)
        nRDF.Add(xa3)

        Dim nFeature As XElement
        Dim nSurface As XElement
        Dim nSketch As XElement
        Dim nPara As XElement
        Dim nRef As XElement


        Dim aAbout As XAttribute
        Dim aLAbel As XAttribute
        Dim aRes As XAttribute
        Dim aValue(50) As XAttribute

        Dim objPlaneNorm(3) As Double
        Dim objPlaneRoot(3) As Double

        Dim objFaceRoot(2) As Double
        Dim objFaceRoot1(2) As Double
        Dim objFaceRoot2(2) As Double
        Dim objFaceNorm(2) As Double

        For i = 1 To objFeatures.Count
            objFeature = objFeatures.Item(i)


            Select Case objFeature.type

                Case SolidEdgePart.FeatureTypeConstants.igExtrudedProtrusionFeatureObject

                    objExtrudedProtrusion = objFeature
                    'Retrieve feature Extent Type, by compare ExtentType() to FeaturePropertyConstants
                    objFeatureExtentType = objExtrudedProtrusion.ExtentType
                    'Retrieve feature Extent Side, by compare ExtentSide() to FeaturePropertyConstants
                    objFeatureExtentSide = objExtrudedProtrusion.ExtentSide

                    Select Case objFeatureExtentType

                        'igFinite: Protrusion with depth.
                        Case SolidEdgeConstants.FeaturePropertyConstants.igFinite
                            'Add the FeatureName
                            objFeatureName = "Feature_" & objExtrudedProtrusion.Name.Split("_")(0) & "_" & objExtrudedProtrusion.Name.Split("_")(1)
                            nFeature = New XElement(kb + "Feature_ExtrudedProtrusion")
                            nRDF.Add(nFeature)
                            aAbout = New XAttribute(kb + "about", "&kb;" & objFeatureName)
                            aLAbel = New XAttribute(rdfs + "label", objFeatureName)
                            nFeature.Add(aAbout)
                            nFeature.Add(aLAbel)

                            objExtrudedProtrusion.ShowDimensions() = True

                            'Add profile plane as FromPlane
                            objProfile = objExtrudedProtrusion.Profile
                            objPlane = objProfile.Plane
                            Call addsurface(objPlane, SurfaceIndex, nRDF, nFeature, "From")

                            'Add Sketch attribute, Circle, Line and Arc
                            nSketch = addsketch(objProfile, SketchIndex, nRDF, nFeature)

                            objCirs = objProfile.Circles2d
                            objLines2D = objProfile.Lines2d
                            objArcs2D = objProfile.Arcs2d

                            For j = 0 To objLines2D.count - 1
                                Call addline(objLines2D, objProfile, j, nSketch)
                            Next
                            For j = 0 To objCirs.Count - 1
                                Call addCircle(objCirs, objProfile, j, nSketch)
                            Next
                            For j = 0 To objArcs2D.Count - 1
                                Call addArc(objArcs2D, objProfile, j, nSketch)
                            Next

                            'Add parameter attribute
                            objParameter = objExtrudedProtrusion.Depth
                            Call addparameter(objFeatureExtentSide, objParameter, ParaIndex, nRDF, nFeature)

                            'Add Boolean Attribute
                            Call addboolean(BoolIndex, nRDF, nFeature, "Union")

                            'Three side: igSymmetric, igLeft, igRight
                            If objFeatureExtentSide = SolidEdgeConstants.FeaturePropertyConstants.igSymmetric Then
                                Call addboolean(BoolIndex, nRDF, nFeature, "TwoDirection")
                            Else : Call addboolean(BoolIndex, nRDF, nFeature, "OneDirection")
                            End If


                            MsgBox(objFeatureName)
                            objExtrudedProtrusion.ShowDimensions() = False

                            'igFromTo: Protrusion from one plane to another
                        Case SolidEdgeConstants.FeaturePropertyConstants.igFromTo
                            objFeatureName = "Feature_" & objExtrudedProtrusion.Name.Split("_")(0) & "_" & objExtrudedProtrusion.Name.Split("_")(1)
                            nFeature = New XElement(kb + "Feature_ExtrudedProtrusion")
                            nRDF.Add(nFeature)
                            aAbout = New XAttribute(kb + "about", "&kb;" & objFeatureName)
                            aLAbel = New XAttribute(rdfs + "label", objFeatureName)
                            nFeature.Add(aAbout)
                            nFeature.Add(aLAbel)

                            objExtrudedProtrusion.ShowDimensions() = True

                            'Add FromPlane
                            objPlane = objExtrudedProtrusion.FromPlane
                            Call addsurface(objPlane, SurfaceIndex, nRDF, nFeature, "From")

                            'Add Sketch
                            objProfile = objExtrudedProtrusion.Profile
                            nSketch = addsketch(objProfile, SketchIndex, nRDF, nFeature)
                            objCirs = objProfile.Circles2d
                            objLines2D = objProfile.Lines2d
                            objArcs2D = objProfile.Arcs2d

                            For j = 0 To objLines2D.count - 1
                                Call addline(objLines2D, objProfile, j, nSketch)
                            Next
                            For j = 0 To objCirs.Count - 1
                                Call addCircle(objCirs, objProfile, j, nSketch)
                            Next
                            For j = 0 To objArcs2D.Count - 1
                                Call addArc(objArcs2D, objProfile, j, nSketch)
                            Next

                            'Add ToPlane
                            objPlane = objExtrudedProtrusion.ToPlane
                            Call addsurface(objPlane, SurfaceIndex, nRDF, nFeature, "To")

                            'Add Boolean
                            Call addboolean(BoolIndex, nRDF, nFeature, "Union")

                            'Add Direction
                            If objFeatureExtentSide = SolidEdgeConstants.FeaturePropertyConstants.igSymmetric Then
                                Call addboolean(BoolIndex, nRDF, nFeature, "TwoDirection")
                            Else : Call addboolean(BoolIndex, nRDF, nFeature, "OneDirection")
                            End If


                            MsgBox(objFeatureName)
                            objExtrudedProtrusion.ShowDimensions() = False

                            'igThroughAll: Protrusion through all
                        Case SolidEdgeConstants.FeaturePropertyConstants.igThroughAll

                            'igToNext: Protrusion to next plane
                        Case SolidEdgeConstants.FeaturePropertyConstants.igToNext
                            objFeatureName = "Feature_" & objExtrudedProtrusion.Name.Split("_")(0) & "_" & objExtrudedProtrusion.Name.Split("_")(1)
                            nFeature = New XElement(kb + "Feature_ExtrudedProtrusion")
                            nRDF.Add(nFeature)
                            aAbout = New XAttribute(kb + "about", "&kb;" & objFeatureName)
                            aLAbel = New XAttribute(rdfs + "label", objFeatureName)
                            nFeature.Add(aAbout)
                            nFeature.Add(aLAbel)

                            objExtrudedProtrusion.ShowDimensions() = True

                            'Add profile plane as FromPlane
                            objProfile = objExtrudedProtrusion.Profile
                            objPlane = objProfile.Plane
                            Call addsurface(objPlane, SurfaceIndex, nRDF, nFeature, "From")

                            'Add Sketch
                            nSketch = addsketch(objProfile, SketchIndex, nRDF, nFeature)
                            objCirs = objProfile.Circles2d
                            objLines2D = objProfile.Lines2d
                            objArcs2D = objProfile.Arcs2d
                            For j = 0 To objLines2D.count - 1
                                Call addline(objLines2D, objProfile, j, nSketch)
                            Next
                            For j = 0 To objCirs.Count - 1
                                Call addCircle(objCirs, objProfile, j, nSketch)
                            Next
                            For j = 0 To objArcs2D.Count - 1
                                Call addArc(objArcs2D, objProfile, j, nSketch)
                            Next

                            'Add Boolean
                            Call addboolean(BoolIndex, nRDF, nFeature, "Union")
                            'Express ToNext by Boolean, ToPlane as an artificial plane
                            Call addboolean(BoolIndex, nRDF, nFeature, "ToNext")

                            'Add a_reference
                            Call objPlane.GetNormal(objPlaneNorm)
                            Call objPlane.GetRootPoint(objPlaneRoot)
                            If objExtrudedProtrusion.ExtentSide = SolidEdgePart.FeaturePropertyConstants.igLeft Then
                                objPlanes = objExtrudedProtrusion.FacesByRay(objPlaneRoot(0), objPlaneRoot(1), objPlaneRoot(2), -objPlaneNorm(0), -objPlaneNorm(1), -objPlaneNorm(2))
                            ElseIf objExtrudedProtrusion.ExtentSide = SolidEdgePart.FeaturePropertyConstants.igRight Then
                                objPlanes = objExtrudedProtrusion.FacesByRay(objPlaneRoot(0), objPlaneRoot(1), objPlaneRoot(2), objPlaneNorm(0), objPlaneNorm(1), objPlaneNorm(2))

                            End If
                            'for anchor joint, item4 is which we need got from testing.
                            objPlane = objPlanes.item(4)

                            objPlane.vertices.item(1).getpointdata(objFaceRoot1)
                            objPlane.vertices.item(3).getpointdata(objFaceRoot2)

                            For j = 0 To 2
                                objFaceRoot(j) = objFaceRoot1(j) / 2 + objFaceRoot2(j) / 2
                            Next

                            Call objPlane.getnormal(1, objFaceRoot, objFaceNorm)

                            'ToNext need a_reference
                            nRef = New XElement(kb + "a_reference")
                            aRes = New XAttribute(rdf + "resource", "&kb;Surface_" & SurfaceIndex)
                            nFeature.Add(nRef)
                            nRef.Add(aRes)

                            nSurface = New XElement(kb + "Surface")
                            aAbout = New XAttribute(rdf + "about", "&kb;Surface_" & SurfaceIndex)
                            aLAbel = New XAttribute(rdfs + "label", "Surface_" & SurfaceIndex)
                            nRDF.Add(nSurface)
                            nSurface.Add(aAbout)
                            nSurface.Add(New XAttribute(kb + "Type", "To"))
                            nSurface.Add(New XAttribute(kb + "RootX", objFaceRoot(0)))
                            nSurface.Add(New XAttribute(kb + "RootY", objFaceRoot(1)))
                            nSurface.Add(New XAttribute(kb + "RootZ", objFaceRoot(2)))
                            nSurface.Add(New XAttribute(kb + "NormX", objFaceNorm(0)))
                            nSurface.Add(New XAttribute(kb + "NormY", objFaceNorm(1)))
                            nSurface.Add(New XAttribute(kb + "NormZ", objFaceNorm(2)))
                            nSurface.Add(aLAbel)
                            SurfaceIndex += 1

                            'Add Direction
                            If objFeatureExtentSide = SolidEdgeConstants.FeaturePropertyConstants.igSymmetric Then
                                Call addboolean(BoolIndex, nRDF, nFeature, "TwoDirection")
                            Else : Call addboolean(BoolIndex, nRDF, nFeature, "OneDirection")
                            End If


                            MsgBox(objFeatureName)
                            objExtrudedProtrusion.ShowDimensions() = False

                            'igToKeyPoint: Protrusion to a keypoint
                        Case SolidEdgeConstants.FeaturePropertyConstants.igToKeyPoint

                    End Select

                    'ExtrudedCutout
                Case SolidEdgePart.FeatureTypeConstants.igExtrudedCutoutFeatureObject

                    objExtrudedCutout = objFeature

                    'Retrieve feature Extent Type, by compare ExtentType() to FeaturePropertyConstants
                    objFeatureExtentType = objExtrudedCutout.ExtentType
                    'Retrieve feature Extent Side, by compare ExtentSide() to FeaturePropertyConstants
                    objFeatureExtentSide = objExtrudedCutout.ExtentSide

                    Select Case objFeatureExtentType

                        'Cutout a depth
                        Case SolidEdgeConstants.FeaturePropertyConstants.igFinite
                            objFeatureName = "Feature_" & objExtrudedCutout.Name.Split("_")(0) & "_" & objExtrudedCutout.Name.Split("_")(1)
                            nFeature = New XElement(kb + "Feature_ExtrudedCutout")
                            nRDF.Add(nFeature)
                            aAbout = New XAttribute(kb + "about", "&kb;" & objFeatureName)
                            aLAbel = New XAttribute(rdfs + "label", objFeatureName)
                            nFeature.Add(aAbout)
                            nFeature.Add(aLAbel)

                            objExtrudedCutout.ShowDimensions() = True

                            'Add FromPlane
                            objProfile = objExtrudedCutout.Profile
                            objPlane = objProfile.Plane
                            Call addsurface(objPlane, SurfaceIndex, nRDF, nFeature, "From")

                            'Add Sketch
                            nSketch = addsketch(objProfile, SketchIndex, nRDF, nFeature)

                            objCirs = objProfile.Circles2d
                            objLines2D = objProfile.Lines2d
                            objArcs2D = objProfile.Arcs2d

                            For j = 0 To objLines2D.count - 1
                                Call addline(objLines2D, objProfile, j, nSketch)
                            Next
                            For j = 0 To objCirs.Count - 1
                                Call addCircle(objCirs, objProfile, j, nSketch)
                            Next
                            For j = 0 To objArcs2D.Count - 1
                                Call addArc(objArcs2D, objProfile, j, nSketch)
                            Next
                            objParameter = objExtrudedCutout.Depth

                            'Add cut inside or outside, ExtentSide not work, all cutouts in anchorjoint are inside. 
                            If objExtrudedCutout.ExtentSide = 1 Then
                                Call addboolean(BoolIndex, nRDF, nFeature, "Inside")
                            End If

                            If objExtrudedCutout.ExtentSide = 2 Then
                                'API does not support ExtentSide well, need more test.
                                'Call addboolean(BoolIndex, nRDF, nFeature, "Outside")
                                Call addboolean(BoolIndex, nRDF, nFeature, "Inside")
                            End If

                            'Add Boolean
                            Call addparameter(objFeatureExtentSide, objParameter, ParaIndex, nRDF, nFeature)
                            Call addboolean(BoolIndex, nRDF, nFeature, "Subtraction")

                            If objFeatureExtentSide = SolidEdgeConstants.FeaturePropertyConstants.igSymmetric Then
                                Call addboolean(BoolIndex, nRDF, nFeature, "TwoDirection")
                            Else : Call addboolean(BoolIndex, nRDF, nFeature, "OneDirection")
                            End If


                            MsgBox(objFeatureName)
                            objExtrudedCutout.ShowDimensions() = False

                            'Cutout from one plane to another
                        Case SolidEdgeConstants.FeaturePropertyConstants.igFromTo
                            objFeatureName = "Feature_" & objExtrudedCutout.Name.Split("_")(0) & "_" & objExtrudedCutout.Name.Split("_")(1)
                            nFeature = New XElement(kb + "Feature_ExtrudedCutout")
                            nRDF.Add(nFeature)
                            aAbout = New XAttribute(kb + "about", "&kb;" & objFeatureName)
                            aLAbel = New XAttribute(rdfs + "label", objFeatureName)
                            nFeature.Add(aAbout)
                            nFeature.Add(aLAbel)

                            objExtrudedCutout.ShowDimensions() = True

                            'Add FromPlane
                            objPlane = objExtrudedCutout.FromPlane
                            Call addsurface(objPlane, SurfaceIndex, nRDF, nFeature, "From")

                            'Add Sketch
                            objProfile = objExtrudedCutout.Profile
                            nSketch = addsketch(objProfile, SketchIndex, nRDF, nFeature)

                            objCirs = objProfile.Circles2d
                            objLines2D = objProfile.Lines2d
                            objArcs2D = objProfile.Arcs2d

                            For j = 0 To objLines2D.count - 1
                                Call addline(objLines2D, objProfile, j, nSketch)
                            Next
                            For j = 0 To objCirs.Count - 1
                                Call addCircle(objCirs, objProfile, j, nSketch)
                            Next
                            For j = 0 To objArcs2D.Count - 1
                                Call addArc(objArcs2D, objProfile, j, nSketch)
                            Next


                            'Add Inside
                            If objExtrudedCutout.ExtentSide = 1 Then
                                Call addboolean(BoolIndex, nRDF, nFeature, "Inside")
                            End If

                            If objExtrudedCutout.ExtentSide = 2 Then
                                'API does not support ExtentSide well, need more test.
                                'Call addboolean(BoolIndex, nRDF, nFeature, "Outside")
                                Call addboolean(BoolIndex, nRDF, nFeature, "Inside")
                            End If

                            'Add ToPlane
                            objPlane = objExtrudedCutout.ToPlane
                            Call addsurface(objPlane, SurfaceIndex, nRDF, nFeature, "To")

                            'Add Boolean
                            Call addboolean(BoolIndex, nRDF, nFeature, "Subtraction")

                            If objFeatureExtentSide = SolidEdgeConstants.FeaturePropertyConstants.igSymmetric Then
                                Call addboolean(BoolIndex, nRDF, nFeature, "TwoDirection")
                            Else : Call addboolean(BoolIndex, nRDF, nFeature, "OneDirection")
                            End If


                            MsgBox(objFeatureName)
                            objExtrudedCutout.ShowDimensions() = False

                        Case SolidEdgeConstants.FeaturePropertyConstants.igThroughAll

                        Case SolidEdgeConstants.FeaturePropertyConstants.igToNext

                        Case SolidEdgeConstants.FeaturePropertyConstants.igToKeyPoint

                    End Select

                    'Mirror
                Case SolidEdgePart.FeatureTypeConstants.igMirrorCopyFeatureObject
                    objMirrorCopy = objFeature
                    objFeatureName = "Feature_" & objMirrorCopy.Name

                    nFeature = New XElement(kb + "Feature_MirrorCopy")
                    nRDF.Add(nFeature)
                    aAbout = New XAttribute(kb + "about", "&kb;" & objFeatureName)
                    aLAbel = New XAttribute(rdfs + "label", objFeatureName)
                    nFeature.Add(aAbout)
                    nFeature.Add(aLAbel)

                    'objMirrorCopy.ShowDimensions() = True
                    objMirrorCopy.Suppress = True

                    'Add pattern plane
                    objPlane = objMirrorCopy.PatternPlane
                    Call addsurface(objPlane, SurfaceIndex, nRDF, nFeature, "Pattern")

                    'Get mirrored features
                    Call objMirrorCopy.GetInputFeatures(objInputFeature)

                    For j = 0 To objInputFeature.Count - 1

                        'Keep feature name
                        Try
                            Dim xx As String = objInputFeature(j).Name()
                            nRef = New XElement(kb + "reference")
                            aRes = New XAttribute(rdf + "resource", "&kb;Feature_" & objInputFeature(j).Name)
                            nFeature.Add(nRef)
                            nRef.Add(aRes)
                        Catch ex As Exception
                            Continue For
                        End Try

                    Next

                    'objMirrorCopy.ShowDimensions() = False
                    objMirrorCopy.Suppress() = False
                    MsgBox(objFeatureName)


                    'Round not finished, too complex.
                Case SolidEdgePart.FeatureTypeConstants.igRoundFeatureObject '462094738

                    objRound = objFeature
                    objFeatureName = "Feature_" & objRound.Name

                    nFeature = New XElement(kb + "Feature_Round")
                    nRDF.Add(nFeature)
                    aAbout = New XAttribute(kb + "about", "&kb;" & objFeatureName)
                    aLAbel = New XAttribute(rdfs + "label", objFeatureName)
                    nFeature.Add(aAbout)
                    nFeature.Add(aLAbel)

                    Dim objLS(0 To 8, 0 To 2) As Double
                    Dim objLE(0 To 8, 0 To 2) As Double
                    Dim objLS1(0 To 2) As Double
                    Dim objLE1(0 To 2) As Double
                    Dim objLS2(0 To 2) As Double
                    Dim objLE2(0 To 2) As Double
                    Dim objEdge(0 To 8) As SolidEdgeGeometry.Edge
                    Dim objEdgePoint1(0 To 2) As Double
                    Dim objEdgePoint2(0 To 2) As Double
                    Dim objRoundRadiiCount As Long
                    Dim objRoundRadii(0 To 2) As Double

                    Call objRound.GetConstantRadii(objRoundRadiiCount, objRoundRadii)
                    'Get the information of Edges

                    For j = 0 To objRound.Edges(3).count / 2 - 1
                        objEdge(2 * j + 1) = objRound.Edges(3).Item(2 * j + 1)
                        Call objEdge(2 * j + 1).GetEndPoints(objLS1, objLE1)
                        objEdge(2 * j + 2) = objRound.Edges(3).Item(2 * j + 2)
                        Call objEdge(2 * j + 2).GetEndPoints(objLS2, objLE2)
                        If (objLS1(0) = objLE2(0)) * (objLE1(0) = objLS2(0)) Then
                            objEdgePoint1(0) = (objLS1(0) + objLS2(0)) / 2
                            objEdgePoint2(0) = (objLS1(0) + objLS2(0)) / 2
                            objEdgePoint1(1) = objLS1(1)
                            objEdgePoint2(1) = objLS2(1)
                            objEdgePoint1(2) = objLS2(2)
                            objEdgePoint2(2) = objLS1(2)
                        End If
                        If (objLS1(1) = objLE2(1)) * (objLE1(1) = objLS2(1)) Then
                            objEdgePoint1(1) = (objLS1(1) + objLS2(1)) / 2
                            objEdgePoint2(1) = (objLS1(1) + objLS2(1)) / 2
                            objEdgePoint1(0) = objLS1(0)
                            objEdgePoint2(0) = objLS2(0)
                            objEdgePoint1(2) = objLS2(2)
                            objEdgePoint2(2) = objLS1(2)
                        End If
                        If (objLS1(2) = objLE2(2)) * (objLE1(2) = objLS2(2)) Then
                            objEdgePoint1(2) = (objLS1(2) + objLS2(2)) / 2
                            objEdgePoint2(2) = (objLS1(2) + objLS2(2)) / 2
                            objEdgePoint1(1) = objLS1(1)
                            objEdgePoint2(1) = objLS2(1)
                            objEdgePoint1(0) = objLS2(0)
                            objEdgePoint2(0) = objLS1(0)
                        End If
                        nRef = New XElement(kb + "reference")
                        aRes = New XAttribute(rdf + "resource", "&kb;Line_" & LineIndex)
                        nFeature.Add(nRef)
                        nRef.Add(aRes)
                        nPara = New XElement(kb + "Line")
                        aAbout = New XAttribute(rdf + "about", "&kb;Line_" & LineIndex)
                        aLAbel = New XAttribute(rdfs + "label", "Line_" & LineIndex)
                        nRDF.Add(nPara)
                        nPara.Add(aAbout)
                        nPara.Add(New XAttribute(kb + "Type", "Edge"))
                        nPara.Add(New XAttribute(kb + "P1X", objEdgePoint1(0)))
                        nPara.Add(New XAttribute(kb + "P1Y", objEdgePoint1(1)))
                        nPara.Add(New XAttribute(kb + "P1Z", objEdgePoint1(2)))
                        nPara.Add(New XAttribute(kb + "P2X", objEdgePoint2(0)))
                        nPara.Add(New XAttribute(kb + "P2Y", objEdgePoint2(1)))
                        nPara.Add(New XAttribute(kb + "P2Z", objEdgePoint2(2)))
                        nPara.Add(aLAbel)
                        LineIndex += 1

                    Next


                    nRef = New XElement(kb + "reference")
                    aRes = New XAttribute(rdf + "resource", "&kb;Parameter_" & ParaIndex)
                    nFeature.Add(nRef)
                    nRef.Add(aRes)
                    nPara = New XElement(kb + "Parameter")
                    aAbout = New XAttribute(rdf + "about", "&kb;Parameter_" & ParaIndex)
                    aLAbel = New XAttribute(rdfs + "label", "Parameter_" & ParaIndex)
                    nRDF.Add(nPara)
                    nPara.Add(aAbout)
                    nPara.Add(New XAttribute(kb + "Type", "Radii"))
                    nPara.Add(New XAttribute(kb + "Radii", objRoundRadii(0)))
                    nPara.Add(aLAbel)
                    ParaIndex += 1


            End Select


        Next

        'Close the doc.
        Call objDoc.Close()
        CheckCreate.Checked = True

    End Sub

    'Add line as part of the sketch
    Sub addline(ByRef objLines2D As SolidEdgeFrameworkSupport.Lines2d, ByRef objProfile As SolidEdgePart.Profile, ByVal j As Integer, ByRef nSketch As XElement)
        Dim objLine As Object
        objLine = New SELine(objLines2D.Item(j + 1), objProfile)
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim nSketchLine = New XElement(kb + ("SketchLine_" & j + 1))
        nSketch.Add(nSketchLine)
        nSketchLine.Add(New XAttribute(kb + "Type", "Line"))
        'keep both 2D coordinator and 3D coordinator
        nSketchLine.Add(New XAttribute(kb + "Start2DX", objLine.dbstart(0)))
        nSketchLine.Add(New XAttribute(kb + "Start2DY", objLine.dbstart(1)))
        nSketchLine.Add(New XAttribute(kb + "Start3DX", objLine.dbstart(2)))
        nSketchLine.Add(New XAttribute(kb + "Start3DY", objLine.dbstart(3)))
        nSketchLine.Add(New XAttribute(kb + "Start3DZ", objLine.dbstart(4)))
        nSketchLine.Add(New XAttribute(kb + "End2DX", objLine.dbend(0)))
        nSketchLine.Add(New XAttribute(kb + "End2DY", objLine.dbend(1)))
        nSketchLine.Add(New XAttribute(kb + "End3DX", objLine.dbend(2)))
        nSketchLine.Add(New XAttribute(kb + "End3DY", objLine.dbend(3)))
        nSketchLine.Add(New XAttribute(kb + "End3DZ", objLine.dbend(4)))

    End Sub

    'Add arc as part of the sketch
    Sub addArc(ByRef objArcs2D As SolidEdgeFrameworkSupport.Arcs2d, ByRef objProfile As SolidEdgePart.Profile, ByVal j As Integer, ByRef nSketch As XElement)
        Dim objArc As Object
        objArc = New SEArc(objArcs2D.Item(j + 1), objProfile)
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim nSketchArc = New XElement(kb + ("SketchArc_" & j + 1))
        nSketch.Add(nSketchArc)
        'Three points + center + key point
        nSketchArc.Add(New XAttribute(kb + "Type", "Arc"))
        nSketchArc.Add(New XAttribute(kb + "Radius", objArc.dbRadius))
        nSketchArc.Add(New XAttribute(kb + "Start2DX", objArc.dbStart(0)))
        nSketchArc.Add(New XAttribute(kb + "Start2DY", objArc.dbStart(1)))
        nSketchArc.Add(New XAttribute(kb + "Start3DX", objArc.dbStart(2)))
        nSketchArc.Add(New XAttribute(kb + "Start3DY", objArc.dbStart(3)))
        nSketchArc.Add(New XAttribute(kb + "Start3DZ", objArc.dbStart(4)))
        nSketchArc.Add(New XAttribute(kb + "End2DX", objArc.dbEnd(0)))
        nSketchArc.Add(New XAttribute(kb + "End2DY", objArc.dbEnd(1)))
        nSketchArc.Add(New XAttribute(kb + "End3DX", objArc.dbEnd(2)))
        nSketchArc.Add(New XAttribute(kb + "End3DY", objArc.dbEnd(3)))
        nSketchArc.Add(New XAttribute(kb + "End3DZ", objArc.dbEnd(4)))
        nSketchArc.Add(New XAttribute(kb + "Center2DX", objArc.dbCenter(0)))
        nSketchArc.Add(New XAttribute(kb + "Center2DY", objArc.dbCenter(1)))
        nSketchArc.Add(New XAttribute(kb + "Center3DX", objArc.dbCenter(2)))
        nSketchArc.Add(New XAttribute(kb + "Center3DY", objArc.dbCenter(3)))
        nSketchArc.Add(New XAttribute(kb + "Center3DZ", objArc.dbCenter(4)))
        'nSketchArc.Add(New XAttribute(kb + "Key2DX", objArc.dbKey(0)))
        'nSketchArc.Add(New XAttribute(kb + "Key2DY", objArc.dbKey(1)))
        nSketchArc.Add(New XAttribute(kb + "Key3DX", objArc.dbKey(2)))
        nSketchArc.Add(New XAttribute(kb + "Key3DY", objArc.dbKey(3)))
        nSketchArc.Add(New XAttribute(kb + "Key3DZ", objArc.dbKey(4)))
    End Sub

    'Add circle as part of the sketch
    Sub addCircle(ByRef objCirs As SolidEdgeFrameworkSupport.Circles2d, ByRef objProfile As SolidEdgePart.Profile, ByVal j As Integer, ByRef nSketch As XElement)
        Dim objCir As Object
        objCir = New SECircle(objCirs.Item(j + 1), objProfile)
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim nSketchCir = New XElement(kb + ("SketchCircle_" & j + 1))
        nSketch.Add(nSketchCir)
        'Four points + center
        nSketchCir.Add(New XAttribute(kb + "Type", "Circle"))
        nSketchCir.Add(New XAttribute(kb + "Radius", objCir.dbRadius))
        nSketchCir.Add(New XAttribute(kb + "Start2DX", objCir.dbStart(0)))
        nSketchCir.Add(New XAttribute(kb + "Start2DY", objCir.dbStart(1)))
        nSketchCir.Add(New XAttribute(kb + "Start3DX", objCir.dbStart(2)))
        nSketchCir.Add(New XAttribute(kb + "Start3DY", objCir.dbStart(3)))
        nSketchCir.Add(New XAttribute(kb + "Start3DZ", objCir.dbStart(4)))
        nSketchCir.Add(New XAttribute(kb + "End2DX", objCir.dbEnd(0)))
        nSketchCir.Add(New XAttribute(kb + "End2DY", objCir.dbEnd(1)))
        nSketchCir.Add(New XAttribute(kb + "End3DX", objCir.dbEnd(2)))
        nSketchCir.Add(New XAttribute(kb + "End3DY", objCir.dbEnd(3)))
        nSketchCir.Add(New XAttribute(kb + "End3DZ", objCir.dbEnd(4)))
        nSketchCir.Add(New XAttribute(kb + "Center2DX", objCir.dbCenter(0)))
        nSketchCir.Add(New XAttribute(kb + "Center2DY", objCir.dbCenter(1)))
        nSketchCir.Add(New XAttribute(kb + "Center3DX", objCir.dbCenter(2)))
        nSketchCir.Add(New XAttribute(kb + "Center3DY", objCir.dbCenter(3)))
        nSketchCir.Add(New XAttribute(kb + "Center3DZ", objCir.dbCenter(4)))
        'nSketchCir.Add(New XAttribute(kb + "P12DX", objCir.dbP1(0)))
        'nSketchCir.Add(New XAttribute(kb + "P12DY", objCir.dbP1(1)))
        nSketchCir.Add(New XAttribute(kb + "P13DX", objCir.dbP1(2)))
        nSketchCir.Add(New XAttribute(kb + "P13DY", objCir.dbP1(3)))
        nSketchCir.Add(New XAttribute(kb + "P13DZ", objCir.dbP1(4)))
        'nSketchCir.Add(New XAttribute(kb + "P22DX", objCir.dbP2(0)))
        'nSketchCir.Add(New XAttribute(kb + "P22DY", objCir.dbP2(1)))
        nSketchCir.Add(New XAttribute(kb + "P23DX", objCir.dbP2(2)))
        nSketchCir.Add(New XAttribute(kb + "P23DY", objCir.dbP2(3)))
        nSketchCir.Add(New XAttribute(kb + "P23DZ", objCir.dbP2(4)))
    End Sub

    'Add surface
    Sub addsurface(ByRef objPlane As Object, ByRef surfaceindex As Integer, ByRef nRDF As XElement, ByRef nFeature As XElement, ByVal Typename As String)
        'Normal + Root for a plane
        Dim objSurfaceNorm(3) As Double
        Dim objSurfaceRoot(3) As Double
        Dim rdf As XNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim rdfs As XNamespace = "http://www.w3.org/2000/01/rdf-schema#"
        Call objPlane.GetNormal(objSurfaceNorm)
        Call objPlane.GetRootPoint(objSurfaceRoot)


        Dim nRef = New XElement(kb + "reference")
        Dim aRes = New XAttribute(rdf + "resource", "&kb;Surface_" & surfaceindex)
        nFeature.Add(nRef)
        nRef.Add(aRes)

        Dim nSurface = New XElement(kb + "Surface")
        Dim aAbout = New XAttribute(rdf + "about", "&kb;Surface_" & surfaceindex)
        Dim aLAbel = New XAttribute(rdfs + "label", "Surface_" & surfaceindex)
        nRDF.Add(nSurface)
        nSurface.Add(aAbout)

        'Keep normal + root
        nSurface.Add(New XAttribute(kb + "Type", Typename))
        nSurface.Add(New XAttribute(kb + "RootX", objSurfaceRoot(0)))
        nSurface.Add(New XAttribute(kb + "RootY", objSurfaceRoot(1)))
        nSurface.Add(New XAttribute(kb + "RootZ", objSurfaceRoot(2)))
        nSurface.Add(New XAttribute(kb + "NormX", objSurfaceNorm(0)))
        nSurface.Add(New XAttribute(kb + "NormY", objSurfaceNorm(1)))
        nSurface.Add(New XAttribute(kb + "NormZ", objSurfaceNorm(2)))
        nSurface.Add(aLAbel)
        surfaceindex += 1

    End Sub

    'Add sketch
    Function addsketch(ByRef objProfile As SolidEdgePart.Profile, ByRef sketchindex As Integer, ByRef nRDF As XElement, ByRef nFeature As XElement) As XElement
        Dim rdf As XNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim rdfs As XNamespace = "http://www.w3.org/2000/01/rdf-schema#"

        Dim nRef = New XElement(kb + "reference")
        Dim aRes = New XAttribute(rdf + "resource", "&kb;Sketch_" & sketchindex)
        nFeature.Add(nRef)
        nRef.Add(aRes)

        Dim nSketch = New XElement(kb + "Sketch")
        Dim aAbout = New XAttribute(rdf + "about", "&kb;Sketch_" & sketchindex)
        Dim aLAbel = New XAttribute(rdfs + "label", "Sketch_" & sketchindex)
        nRDF.Add(nSketch)
        nSketch.Add(aAbout)
        nSketch.Add(aLAbel)
        sketchindex += 1

        Return nSketch
    End Function

    'Add parameter
    Sub addparameter(ByVal objFeatureExtentSide As Integer, ByVal objParameter As Double, ByRef ParaIndex As Integer, ByRef nRDF As XElement, ByRef nFeature As XElement)
        Dim objDepthLeft As Double
        Dim objDepthRight As Double
        Dim rdf As XNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim rdfs As XNamespace = "http://www.w3.org/2000/01/rdf-schema#"
        'Assign the Depth parameter

        Dim nRef As XElement
        Dim aRes As XAttribute
        Dim nPara As XElement
        Dim aAbout As XAttribute
        Dim aLabel As XAttribute


        Select Case objFeatureExtentSide
            'Extent to left
            Case SolidEdgeConstants.FeaturePropertyConstants.igLeft
                objDepthLeft = objParameter
                objDepthRight = 0
                nRef = New XElement(kb + "reference")
                aRes = New XAttribute(rdf + "resource", "&kb;Parameter_" & ParaIndex)
                nFeature.Add(nRef)
                nRef.Add(aRes)
                nPara = New XElement(kb + "Parameter")
                aAbout = New XAttribute(rdf + "about", "&kb;Parameter_" & ParaIndex)
                aLabel = New XAttribute(rdfs + "label", "Parameter_" & ParaIndex)
                nRDF.Add(nPara)
                nPara.Add(aAbout)
                nPara.Add(New XAttribute(kb + "Type", "Depth"))
                nPara.Add(New XAttribute(kb + "Left", objDepthLeft))
                nPara.Add(aLabel)
                ParaIndex += 1

                'Extent to right
            Case SolidEdgeConstants.FeaturePropertyConstants.igRight
                objDepthLeft = 0
                objDepthRight = objParameter
                nRef = New XElement(kb + "reference")
                aRes = New XAttribute(rdf + "resource", "&kb;Parameter_" & ParaIndex)
                nFeature.Add(nRef)
                nRef.Add(aRes)
                nPara = New XElement(kb + "Parameter")
                aAbout = New XAttribute(rdf + "about", "&kb;Parameter_" & ParaIndex)
                aLabel = New XAttribute(rdfs + "label", "Parameter_" & ParaIndex)
                nRDF.Add(nPara)
                nPara.Add(aAbout)
                nPara.Add(New XAttribute(kb + "Type", "Depth"))
                nPara.Add(New XAttribute(kb + "Right", objDepthRight))
                nPara.Add(aLabel)
                ParaIndex += 1

                'Symmetric extension
            Case SolidEdgeConstants.FeaturePropertyConstants.igSymmetric
                objDepthLeft = objParameter / 2
                objDepthRight = objParameter / 2
                nRef = New XElement(kb + "reference")
                aRes = New XAttribute(rdf + "resource", "&kb;Parameter_" & ParaIndex)
                nFeature.Add(nRef)
                nRef.Add(aRes)
                nPara = New XElement(kb + "Parameter")
                aAbout = New XAttribute(rdf + "about", "&kb;Parameter_" & ParaIndex)
                aLabel = New XAttribute(rdfs + "label", "Parameter_" & ParaIndex)
                nRDF.Add(nPara)
                nPara.Add(aAbout)
                nPara.Add(New XAttribute(kb + "Type", "Depth"))
                nPara.Add(New XAttribute(kb + "Left", objDepthLeft))
                nPara.Add(aLabel)
                ParaIndex += 1
                nRef = New XElement(kb + "reference")
                aRes = New XAttribute(rdf + "resource", "&kb;Parameter_" & ParaIndex)
                nFeature.Add(nRef)
                nRef.Add(aRes)
                nPara = New XElement(kb + "Parameter")
                aAbout = New XAttribute(rdf + "about", "&kb;Parameter_" & ParaIndex)
                aLabel = New XAttribute(rdfs + "label", "Parameter_" & ParaIndex)
                nRDF.Add(nPara)
                nPara.Add(aAbout)
                nPara.Add(New XAttribute(kb + "Type", "Depth"))
                nPara.Add(New XAttribute(kb + "Right", objDepthRight))
                nPara.Add(aLabel)
                ParaIndex += 1
        End Select

    End Sub

    'Add Boolean
    Sub addboolean(ByRef BoolIndex As Integer, ByRef nRDF As XElement, ByRef nFeature As XElement, ByVal BoolType As String)

        Dim rdf As XNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim rdfs As XNamespace = "http://www.w3.org/2000/01/rdf-schema#"

        Dim nRef = New XElement(kb + "reference")
        Dim aRes = New XAttribute(rdf + "resource", "&kb;BooleanSign_" & BoolIndex)
        nFeature.Add(nRef)
        nRef.Add(aRes)

        Dim nBool = New XElement(kb + "BooleanSign")
        Dim aAbout = New XAttribute(rdf + "about", "&kb;BooleanSign_" & BoolIndex)
        Dim aLAbel = New XAttribute(rdfs + "label", "BooleanSign_" & BoolIndex)
        nRDF.Add(nBool)
        nBool.Add(aAbout)
        nBool.Add(New XAttribute(kb + "Type", BoolType))
        nBool.Add(aLAbel)
        BoolIndex += 1
    End Sub

    'Save RDF file
    Public Sub SaveRDF_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveRDF.Click
        If SaveRDF1.ShowDialog() = DialogResult.OK Then
            Dim myStreamWriter As New StreamWriter(SaveRDF1.FileName)
            If Not (myStreamWriter Is Nothing) Then
                myStreamWriter.Close()
                Dim dTaskID As Double, path As String
                rfile.Save(SaveRDF1.FileName)
                path = "C:\Program Files\Internet Explorer\iexplore.exe"
                dTaskID = Shell(path + " " + SaveRDF1.FileName, vbNormalFocus)
                CheckSave.Checked = True
                Me.Close()
                Form1.Visible = True
            End If
        End If
    End Sub
End Class