'This is build a 3D model in SolidWorks by using the RDF file generated in DynamicMapping.
'The version of SolidWorks is 2007 sp0.0
'The basic thinking behind codes is extract all attribute information of a feature then create them one by one.
'And the last step is to create the whole model.
'For instance, the extrude of a depth is create sketch first, select the sketch plane, get the depth, and then extrude.
'Hence the sequence of attributes matters.
'In the last, the new model will be saved as a .sldprt file and can be read by SolidWorks directly.

Imports System
Imports System.IO
Imports System.Xml.Linq
Imports System.Xml
Imports <xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#">
Imports <xmlns:kb="http://www.e-designcenter.info/kb#">
Imports <xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#">


Public Class RDFtoSW
    Dim SWFileName As String

    Dim Part As Object

    Private Sub OpenRDF2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenRDF2.Click
        If OpenRDF.ShowDialog() = DialogResult.OK Then
            Dim myStreamReader As New StreamReader(OpenRDF.FileName)
            SWFileName = OpenRDF.FileName
            'Dim FExt As String = OpenRDF.DefaultExt
            myStreamReader.Close()
            RDF2Check.Checked = True
        End If
    End Sub


    Private Sub CreateSW_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CreateSW.Click
        Dim file As XDocument = XDocument.Load(SWFileName)
        'Dim FeatureFullName As String
        Dim FeatureName As String

        'Keyword to check feature name
        Dim xExtrude As String = "Extrude"
        Dim xCutExtrude As String = "CutExtrude"
        Dim xMirror As String = "Mirror"
        Dim xFillet As String = "Round"

        Dim swApp As SldWorks.SldWorks
        Dim SelMgr As Object
        Dim boolstatus As Boolean
        Dim longstatus As Long, longwarnings As Long
        Dim Feature As Object
        Dim docext As SldWorks.ModelDocExtension

        Dim SketchIndex As Integer = 1
        Dim SurfaceIndex As Integer = 1

        'Start SolidWorks
        Try
            swApp = GetObject(, "SldWorks.Application")
            swApp.Visible = True
        Catch ex As Exception
            swApp = CreateObject("SldWorks.Application")
            swApp.Visible = True
        End Try

        'Create new document
        Part = swApp.NewDocument("D:\Program Files\SolidWorks\data\templates\Part.prtdot", 0, 0.0#, 0.0#)
        Part = swApp.ActivateDoc("")
        docext = Part.extension


        Dim query = From p In file.Descendants _
        Where p.Name.ToString.Contains("Feature") _
        Select p

        'Feature query
        For Each record In query
            FeatureName = record.@rdfs:label
            'Console.WriteLine(FeatureName)

            Dim query1 = From pp In record...<kb:reference> _
            Select pp.@rdf:resource.Split(";")(1)

            If FeatureName.Contains(xExtrude) Then
                If FeatureName.Contains(xCutExtrude) Then
                    Call AddCutout(FeatureName, file, docext, Part, SketchIndex, SurfaceIndex)
                Else
                    Call AddExtrude(FeatureName, file, docext, Part, SketchIndex, SurfaceIndex)
                End If
            End If

            'If FeatureName.Contains(xCutExtrude) Then
            '    Call AddCutout(FeatureName, file, docext, Part, SketchIndex, SurfaceIndex)
            'End If

                If FeatureName.Contains(xMirror) Then
                    Call AddMirror(FeatureName, file, docext, Part, SurfaceIndex, xExtrude, xCutExtrude)
                End If

                If FeatureName.Contains(xFillet) Then
                    Call AddFillet(FeatureName, file, docext, Part)
                End If
        Next

        SWCheck.Checked = True
        'Console.ReadLine()
    End Sub

    Sub AddExtrude(ByVal FeatureName As String, ByRef file As XDocument, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object, ByRef SketchIndex As Integer, ByRef SurfaceIndex As Integer)
        Dim i As Integer
        Dim AttrCount As Integer
        Dim AttrName(8) As String
        Dim PlaneName As String
        Dim PlaneRoot(2) As Double

        Dim FromPlaneName As String
        Dim FromPlaneRoot(2) As Double
        Dim ToPlaneName As String
        Dim ToPlaneRoot(2) As Double
        Dim SurfaceType As String
        Dim SketchName As String
        Dim DepthLeft As Double
        Dim DepthRight As Double
        Dim boolstatus As Boolean

        Dim FeatureQuery = From p In file.Descendants _
                    Where p.@rdfs:label = FeatureName _
                    Select p...<kb:reference>

        'Get Attributes information
        For Each record In FeatureQuery
            AttrCount = record.Count
            For i = 1 To record.Count
                AttrName(i - 1) = record(i - 1).@rdf:resource.ToString.Split(";")(1)
            Next
        Next

        For i = 1 To AttrCount
            If AttrName(i - 1).Contains("Sketch") Then
                Call AddSketch(AttrName(i - 1), file, docext, part, SketchIndex, SketchName)

            End If
            If AttrName(i - 1).Contains("Surface") Then
                Call AddSurface(AttrName(i - 1), file, docext, part, SurfaceIndex, FromPlaneName, FromPlaneRoot, ToPlaneName, ToPlaneRoot, SurfaceType)

                'Create extrution if contains surface_to
                If SurfaceType = "To" Then
                    part.ClearSelection2(True)

                    'The sequence of selection is very important.
                    boolstatus = docext.SelectByID2(SketchName, "SKETCH", 0, 0, 0, False, 0, Nothing, 0)
                    'boolstatus = docext.SelectByID2(FromPlaneName, "PLANE", FromPlaneRoot(0), FromPlaneRoot(1), FromPlaneRoot(2), True, 1, Nothing, 0)
                    'boolstatus = docext.SelectByID2(ToPlaneName, "PLANE", ToPlaneRoot(0), ToPlaneRoot(1), ToPlaneRoot(2), True, 1, Nothing, 0)

                    'boolstatus = docext.SelectByID2(ToPlaneName, "PLANE", ToPlaneRoot(0), ToPlaneRoot(1), ToPlaneRoot(2), True, 1, Nothing, 0)
                    'boolstatus = docext.SelectByID2("Front Plane", "PLANE", 0, 0, 0, True, 1, Nothing, 0)
                    boolstatus = docext.SelectByID2("", "FACE", ToPlaneRoot(0), ToPlaneRoot(1), ToPlaneRoot(2), True, 1, Nothing, 0)
                    boolstatus = docext.SelectByID2(FromPlaneName, "PLANE", FromPlaneRoot(0), FromPlaneRoot(1), FromPlaneRoot(2), True, 16, Nothing, 0)


                    part.FeatureManager.FeatureExtrusion(True, False, True, 4, 0, 0.04, 0.04, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 1, 1, 1)

                    part.SelectionManager.EnableContourSelection = 0
                End If

            End If

            'Do extrusion if contains parameter
            If AttrName(i - 1).Contains("Parameter") Then
                Call AddParameter(AttrName(i - 1), file, docext, part, DepthLeft, DepthRight)


                If DepthLeft <> 0 And DepthRight = 0 Then
                    part.ClearSelection2(True)
                    boolstatus = docext.SelectByID2(SketchName, "SKETCH", 0, 0, 0, False, 0, Nothing, 0)
                    boolstatus = docext.SelectByID2(FromPlaneName, "PLANE", FromPlaneRoot(0), FromPlaneRoot(1), FromPlaneRoot(2), True, 16, Nothing, 0)

                    'part.FeatureManager.FeatureExtrusion2(False, False, True, 0, 0, DepthLeft, DepthRight, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 1, 1, 1, 0, 0, False)
                    part.FeatureManager.FeatureExtrusion2(True, False, False, 0, 0, DepthLeft, DepthRight, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 1, 1, 1, 0, 0, False)
                    part.SelectionManager.EnableContourSelection = 0
                End If

                If DepthLeft = 0 And DepthRight <> 0 Then
                    part.ClearSelection2(True)
                    boolstatus = docext.SelectByID2(SketchName, "SKETCH", 0, 0, 0, False, 0, Nothing, 0)
                    boolstatus = docext.SelectByID2(FromPlaneName, "PLANE", FromPlaneRoot(0), FromPlaneRoot(1), FromPlaneRoot(2), True, 16, Nothing, 0)

                    'part.FeatureManager.FeatureExtrusion2(False, False, True, 0, 0, DepthLeft, DepthRight, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 1, 1, 1, 0, 0, False)
                    part.FeatureManager.FeatureExtrusion2(True, False, True, 0, 0, DepthRight, DepthLeft, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 1, 1, 1, 0, 0, False)
                    part.SelectionManager.EnableContourSelection = 0
                End If


                If DepthLeft <> 0 And DepthRight <> 0 Then
                    part.ClearSelection2(True)
                    boolstatus = docext.SelectByID2(SketchName, "SKETCH", 0, 0, 0, False, 0, Nothing, 0)
                    boolstatus = docext.SelectByID2(FromPlaneName, "PLANE", FromPlaneRoot(0), FromPlaneRoot(1), FromPlaneRoot(2), True, 16, Nothing, 0)

                    part.FeatureManager.FeatureExtrusion2(False, False, True, 0, 0, DepthLeft, DepthRight, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 1, 1, 1, 0, 0, False)
                    'part.FeatureManager.FeatureExtrusion2(True, False, False, 0, 0, DepthLeft, DepthRight, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 1, 1, 1, 0, 0, False)
                    part.SelectionManager.EnableContourSelection = 0
                End If
            End If

        Next

    End Sub

    'There are two possible surface configurations, the first one is make of root and norm, the second one is made of three points
    'This function is used to draw surface on SW
    Public Sub AddSurface(ByVal AttrName As String, ByRef file As XDocument, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object, ByRef SurfaceIndex As Integer, _
                               ByRef FromPlaneName As String, ByRef FromPlaneRoot() As Double, ByRef ToPlaneName As String, ByRef ToPlaneRoot() As Double, ByRef SurfaceType As String)
        Dim AttrQuery = From p In file.Descendants _
                        Where p.@rdfs:label = AttrName _
                        Select p
        Dim AttrNode As XElement = AttrQuery.First
        Dim PlaneName As String
        Dim Surface1 As Object
        Dim boolstatus As Boolean
        Surface1 = New AttrSurface

        For Each Attr In AttrQuery
            Surface1.Name = Attr.@rdfs:label
            Surface1.Type = Attr.@kb:Type
            SurfaceType = Surface1.Type
            If Attr.@kb:RootX <> "" Then

                Surface1.Root(0) = Attr.@kb:RootX
                Surface1.Root(1) = Attr.@kb:RootY
                Surface1.Root(2) = Attr.@kb:RootZ
                Surface1.Norm(0) = Attr.@kb:NormX
                Surface1.Norm(1) = Attr.@kb:NormY
                Surface1.Norm(2) = Attr.@kb:NormZ
            Else
                Surface1.Root(0) = Attr.@kb:P1X
                Surface1.Root(1) = Attr.@kb:P1Y
                Surface1.Root(2) = Attr.@kb:P1Z
                Surface1.P1(0) = Attr.@kb:P2X
                Surface1.P1(1) = Attr.@kb:P2Y
                Surface1.P1(2) = Attr.@kb:P2Z
                Surface1.P2(0) = Attr.@kb:P3X
                Surface1.P2(1) = Attr.@kb:P3Y
                Surface1.P2(2) = Attr.@kb:P3Z

            End If
        Next

        If Surface1.Type = "From" Then
            'part.ClearSelection2(True)
            Call Surface1.SelcetSurface(docext, part, SurfaceIndex)
            FromPlaneName = Surface1.PlaneName
            FromPlaneRoot = Surface1.Root
        End If

        If Surface1.Type = "Pattern" Then
            'part.ClearSelection2(True)
            Call Surface1.SelcetSurface(docext, part, SurfaceIndex)
            FromPlaneName = Surface1.PlaneName
            FromPlaneRoot = Surface1.Root
        End If

        If Surface1.Type = "To" Then
            'boolstatus = docext.SelectByID2("3DSketch" & SketchIndex, "SKETCH", 0, 0, 0, False, 0, Nothing, 0)
            'boolstatus = docext.SelectByID2(PlaneName, "PLANE", 0, 0, 0, True, 16, Nothing, 0)
            'PlaneName = Surface1.SelcetSurface(docext, part)
            'boolstatus = docext.SelectByID2(PlaneName, "PLANE", 0, 0, 0, True, 16, Nothing, 0)
            'part.FeatureManager.FeatureExtrusion(True, False, True, 4, 0, 0, 0, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 1, 1, 1)
            'part.SelectionManager.EnableContourSelection = 0
            'part.ClearSelection2(True)
            Call Surface1.SelcetSurface(docext, part, SurfaceIndex)
            ToPlaneName = Surface1.PlaneName
            ToPlaneRoot = Surface1.Root
        End If
    End Sub

    'This function is used to draw lines in SW
    Public Sub AddLine(ByVal AttrName As String, ByRef file As XDocument, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object, _
                       ByRef Point1() As Double, ByRef Point2() As Double, ByRef LineType As String)
        Dim AttrQuery = From p In file.Descendants _
                       Where p.@rdfs:label = AttrName _
                       Select p
        Dim AttrNode As XElement = AttrQuery.First
        Dim Line1 As Object
        Dim boolstatus As Boolean
        Line1 = New AttrLine

        For Each Attr In AttrQuery
            Line1.Name = Attr.@rdfs:label
            Line1.Type = Attr.@kb:Type
            Line1.Point1(0) = Attr.@kb:P1X
            Line1.Point1(1) = Attr.@kb:P1Y
            Line1.Point1(2) = Attr.@kb:P1Z
            Line1.Point2(0) = Attr.@kb:P2X
            Line1.Point2(1) = Attr.@kb:P2Y
            Line1.Point2(2) = Attr.@kb:P2Z
        Next


        If Line1.Type = "Edge" Then
            'part.ClearSelection2(True)
            LineType = "EDGE"
            Call Line1.SelectLine(docext, part)
        End If
        For i = 0 To 2
            Point1(i) = Line1.Point1(i)
            Point2(i) = Line1.Point2(i)
        Next
    End Sub

    'This function is used to draw a 3D sketch in SW, 2D sketch is hard to use.
    Public Sub AddSketch(ByVal AttrName As String, ByRef file As XDocument, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object, ByRef SketchIndex As Integer, ByRef SketchName As String)
        part.Insert3DSketch()
        'part.ClearSelection2(True)

        Dim boolstatus As Boolean
        Dim AttrQuery = From p In file.Descendants _
                Where p.@rdfs:label = AttrName _
                Select p
        Dim AttrNode As XElement = AttrQuery.First
        Dim Sketch1 As Object
        Sketch1 = New AttrSketch

        Dim SketchInfo = From p In AttrNode.Descendants _
                         Select p

        'Only consider three types of sketch. Circle is made by two arcs.
        For Each record In SketchInfo
            If record.@kb:Type = "Line" Then
                Call Sketch1.DrawLine(record, docext, part)
            End If

            If record.@kb:Type = "Arc" Then
                Call Sketch1.DrawArc(record, docext, part)
            End If

            If record.@kb:Type = "Circle" Then
                Call Sketch1.DrawCircle(record, docext, part)
            End If
        Next
        'boolstatus = docext.SelectByID2("3DSketch" & SketchIndex, "SKETCH", 0, 0, 0, True, 16, Nothing, 0)
        SketchName = "3DSketch" & SketchIndex
        SketchIndex += 1
        part.Insert3DSketch()
        part.ClearSelection2(True)
    End Sub

    'This is used to deal with parameters
    Public Sub AddParameter(ByVal AttrName As String, ByRef file As XDocument, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object, ByRef Para1 As Double, ByRef Para2 As Double)
        'Para1 = DepthLeft, Para2 = DepthRight
        'Para2 = Radii, Para2 = Diameter

        Dim AttrQuery = From p In file.Descendants _
        Where p.@rdfs:label = AttrName _
        Select p
        Dim AttrNode As XElement = AttrQuery.First

        If AttrNode.@kb:Type = "Depth" Then
            Para1 = AttrNode.@kb:Left
            Para2 = AttrNode.@kb:Right
        End If

        If AttrNode.@kb:Type = "Radii" Then
            Para1 = AttrNode.@kb:Radii
            Para2 = Para1 * 2
        End If

    End Sub

    'CutSide is not used
    Public Sub AddCutSide(ByVal AttrName As String, ByRef file As XDocument, ByRef docext As SldWorks.ModelDocExtension, ByRef CutSide As Boolean)
        Dim AttrQuery = From p In file.Descendants _
            Where p.@rdfs:label = AttrName _
            Select p
        Dim AttrNode As XElement = AttrQuery.First

        If AttrNode.@kb:Type = "Inside" Then
            CutSide = False
        End If

        If AttrNode.@kb:Type = "Outside" Then
            CutSide = True
        End If

    End Sub

    Sub AddCutout(ByVal FeatureName As String, ByRef file As XDocument, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object, ByRef SketchIndex As Integer, ByRef SurfaceIndex As Integer)
        Dim i As Integer
        Dim AttrCount As Integer
        Dim AttrName(8) As String
        Dim PlaneName As String
        Dim PlaneRoot(2) As Double

        Dim FromPlaneName As String
        Dim FromPlaneRoot(2) As Double
        Dim ToPlaneName As String
        Dim ToPlaneRoot(2) As Double
        Dim SurfaceType As String
        Dim SketchName As String
        Dim DepthLeft As Double
        Dim DepthRight As Double
        Dim boolstatus As Boolean
        Dim Cutside As Boolean

        Dim FeatureQuery = From p In file.Descendants _
                    Where p.@rdfs:label = FeatureName _
                    Select p...<kb:reference>

        For Each record In FeatureQuery
            AttrCount = record.Count
            For i = 1 To record.Count
                Console.WriteLine("test")
                AttrName(i - 1) = record(i - 1).@rdf:resource.ToString.Split(";")(1)
                Console.WriteLine(AttrName(i - 1))
            Next
        Next

        For i = 1 To AttrCount
            If AttrName(i - 1).Contains("Sketch") Then
                Call AddSketch(AttrName(i - 1), file, docext, part, SketchIndex, SketchName)

            End If

            If AttrName(i - 1).Contains("BooleanSign") Then
                Call AddCutSide(AttrName(i - 1), file, docext, Cutside)
            End If

            If AttrName(i - 1).Contains("Surface") Then
                Call AddSurface(AttrName(i - 1), file, docext, part, SurfaceIndex, FromPlaneName, FromPlaneRoot, ToPlaneName, ToPlaneRoot, SurfaceType)

                If SurfaceType = "To" Then
                    part.ClearSelection2(True)

                    boolstatus = docext.SelectByID2(SketchName, "SKETCH", 0, 0, 0, False, 0, Nothing, 0)
                    'boolstatus = docext.SelectByID2(FromPlaneName, "PLANE", FromPlaneRoot(0), FromPlaneRoot(1), FromPlaneRoot(2), True, 1, Nothing, 0)
                    'boolstatus = docext.SelectByID2(ToPlaneName, "PLANE", ToPlaneRoot(0), ToPlaneRoot(1), ToPlaneRoot(2), True, 1, Nothing, 0)

                    boolstatus = docext.SelectByID2(ToPlaneName, "PLANE", ToPlaneRoot(0), ToPlaneRoot(1), ToPlaneRoot(2), True, 1, Nothing, 0)
                    'boolstatus = docext.SelectByID2("Front Plane", "PLANE", 0, 0, 0, True, 1, Nothing, 0)
                    'boolstatus = docext.SelectByID2("", "Face", ToPlaneRoot(0), ToPlaneRoot(1), ToPlaneRoot(2), True, 1, Nothing, 0)
                    boolstatus = docext.SelectByID2(FromPlaneName, "PLANE", FromPlaneRoot(0), FromPlaneRoot(1), FromPlaneRoot(2), True, 16, Nothing, 0)


                    part.FeatureManager.FeatureCut(True, Cutside, True, 4, 0, 0, 0, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 0, 1, 1)

                    part.SelectionManager.EnableContourSelection = 0
                End If

            End If

            If AttrName(i - 1).Contains("Parameter") Then
                Call AddParameter(AttrName(i - 1), file, docext, part, DepthLeft, DepthRight)

                If DepthLeft = 0 Or DepthRight = 0 Then
                    part.ClearSelection2(True)
                    boolstatus = docext.SelectByID2(SketchName, "SKETCH", 0, 0, 0, False, 0, Nothing, 0)
                    boolstatus = docext.SelectByID2(FromPlaneName, "PLANE", FromPlaneRoot(0), FromPlaneRoot(1), FromPlaneRoot(2), True, 16, Nothing, 0)

                    part.FeatureManager.FeatureCut(True, Cutside, True, 0, 0, DepthLeft + DepthRight, DepthRight + DepthLeft, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 0, 1, 1)

                    'part.FeatureManager.FeatureCut(True, Cutside, True, 0, 0, DepthRight + DepthLeft, 0.062, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 0, 0, 1)
                    'part.FeatureManager.FeatureCut(True, Cutside, False, 0, 0, DepthLeft + DepthRight, 0.062, False, False, False, False, 0.01745329251994, 0.01745329251994, False, False, False, False, 0, 0, 1)

                    part.SelectionManager.EnableContourSelection = 0
                End If

            End If

        Next
    End Sub

    Sub AddMirror(ByVal FeatureName As String, ByRef file As XDocument, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object, ByRef SurfaceIndex As Integer, ByVal xExtrude As String, ByVal xCutExtrude As String)
        Dim i As Integer
        Dim AttrCount As Integer
        Dim AttrName(8) As String
        Dim PlaneName As String
        Dim PlaneRoot(2) As Double

        Dim FromPlaneName As String
        Dim FromPlaneRoot(2) As Double
        Dim ToPlaneName As String
        Dim ToPlaneRoot(2) As Double
        Dim SurfaceType As String
        Dim MirrorFeatureName As String
        Dim boolstatus As Boolean

        Dim FeatureQuery = From p In file.Descendants _
                    Where p.@rdfs:label = FeatureName _
                    Select p...<kb:reference>

        For Each record In FeatureQuery
            AttrCount = record.Count
            For i = 1 To record.Count
                'Console.WriteLine("test")
                AttrName(i - 1) = record(i - 1).@rdf:resource.ToString.Split(";")(1)
                Console.WriteLine(AttrName(i - 1))
            Next
        Next
        part.ClearSelection2(True)
        For i = 1 To AttrCount

            If AttrName(i - 1).Contains("Surface") Then
                Call AddSurface(AttrName(i - 1), file, docext, part, SurfaceIndex, FromPlaneName, FromPlaneRoot, ToPlaneName, ToPlaneRoot, SurfaceType)
                boolstatus = docext.SelectByID2(FromPlaneName, "PLANE", FromPlaneRoot(0), FromPlaneRoot(1), FromPlaneRoot(2), True, 1, Nothing, 0)
            End If

            'Select the features to mirror.
            If AttrName(i - 1).Contains("Feature") Then
                If AttrName(i - 1).Contains(xExtrude) Then
                    MirrorFeatureName = "Extrude" & AttrName(i - 1).Split("_").Last
                End If

                If AttrName(i - 1).Contains(xCutExtrude) Then
                    MirrorFeatureName = "Cut-Extrude" & AttrName(i - 1).Split("_").Last
                End If
                boolstatus = docext.SelectByID2(MirrorFeatureName, "BODYFEATURE", 0, 0, 0, True, 1, Nothing, 0)
            End If
        Next
        part.FeatureManager.InsertMirrorFeature(False, False, False, False)
        part.SelectionManager.EnableContourSelection = 0
    End Sub

    'Fillet not finished.
    Sub AddFillet(ByVal FeatureName As String, ByRef file As XDocument, ByRef docext As SldWorks.ModelDocExtension, ByRef part As Object)
        Dim i As Integer
        Dim AttrCount As Integer
        Dim AttrName(8) As String
        Dim boolstatus As Boolean
        Dim Point1(2) As Double
        Dim Point2(2) As Double
        Dim LineType As String
        Dim Radii As Double
        Dim Diameter As Double

        Dim FeatureQuery = From p In file.Descendants _
                    Where p.@rdfs:label = FeatureName _
                    Select p...<kb:reference>

        For Each record In FeatureQuery
            AttrCount = record.Count
            For i = 1 To record.Count
                'Console.WriteLine("test")
                AttrName(i - 1) = record(i - 1).@rdf:resource.ToString.Split(";")(1)
                Console.WriteLine(AttrName(i - 1))
            Next
        Next

        part.ClearSelection2(True)
        part.ViewDisplayWireframe()
        part.ShowNamedView2("*Trimetric", 8)

        For i = 1 To AttrCount
            If AttrName(i - 1).Contains("Line") Then
                Call AddLine(AttrName(i - 1), file, docext, part, Point1, Point2, LineType)
                boolstatus = docext.SelectByID2("", LineType, Point1(0), Point1(1), Point1(2), True, 1, Nothing, 0)
                boolstatus = docext.SelectByID2("", LineType, Point2(0), Point2(1), Point2(2), True, 1, Nothing, 0)
            End If

            If AttrName(i - 1).Contains("Parameter") Then
                Call AddParameter(AttrName(i - 1), file, docext, part, Radii, Diameter)
                part.FeatureFillet5(195, Radii, 0, 0, 0, 0, 0)
            End If
        Next
        part.ViewDisplayShaded()
    End Sub

    Private Sub SaveSW_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveSW.Click
        If SaveSWPart.ShowDialog() = DialogResult.OK Then
            Dim myStreamWriter As New StreamWriter(SaveSWPart.FileName)
            If Not (myStreamWriter Is Nothing) Then
                myStreamWriter.Close()
                Dim dTaskID As Double, path As String
                Part.SaveAs2(SaveSWPart.FileName, 0, False, False)
                'path = "C:\Program Files\Internet Explorer\iexplore.exe"
                'dTaskID = Shell(path + " " + SaveRDF2.FileName, vbNormalFocus)
                SWPartCheck.Checked = True
                Me.Close()
                Form1.Visible = True
            End If
        End If

    End Sub
End Class