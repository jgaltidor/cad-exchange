'This is the mapping between two RDF files, the RDF model file of one system and library file of another system
'Mapping resuls are shown in the form. A new RDF document are created automatecally, which is the RDF file for the second system.
'The difficulty is how to generate the missing attributes in the library. Here are only contains generate a plane by three points.
'The library RDF file of SolidWorks is created manually.

Imports System
Imports System.IO
Imports System.Xml.Linq
Imports System.Xml
Imports <xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#">
Imports <xmlns:kb="http://www.e-designcenter.info/kb#">
Imports <xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#">

Public Class DynamicMap

    Dim Lib2Name As String
    Dim RDF1Name As String
    Dim nSurface As XElement

    Private Sub D_OpenLib2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles D_OpenLib2.Click
        If OpenLib2.ShowDialog() = DialogResult.OK Then
            Dim myStreamReader As New StreamReader(OpenLib2.FileName)
            Lib2Name = OpenLib2.FileName
            Lib2_Name.Text = Lib2Name.Split("\").Last
            Dim FExt As String = OpenLib2.DefaultExt
            myStreamReader.Close()
            Lib2Check.Checked = True
        End If
    End Sub

    'Open RDF file
    Private Sub D_OpenRDF1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles D_OpenRDF1.Click
        If OpenRDF1.ShowDialog() = DialogResult.OK Then
            Dim myStreamReader As New StreamReader(OpenRDF1.FileName)
            RDF1Name = OpenRDF1.FileName
            RDF1_Name.Text = RDF1Name.Split("\").Last
            Dim FExt As String = OpenRDF1.DefaultExt
            myStreamReader.Close()
            RDF1Check.Checked = True
        End If
    End Sub

    Private Sub D_CreateRDF2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles D_CreateRDF2.Click

        Dim LFile As XDocument = XDocument.Load(Lib2Name)
        Dim MFile As XDocument = XDocument.Load(RDF1Name)

        Dim MFeatureName As String

        Dim Sim As Double = 0
        Dim MapResult As String

        'Query all features
        Dim query = From p In MFile.Descendants _
                    Where p.Name.ToString.Contains("Feature") _
                    Select p


        'For each feature in RDF file
        For Each record In query
            MFeatureName = record.@rdfs:label
            RDF1_Feature.Text = MFeatureName

            MsgBox("Goto next Source")
            'Do mapping
            Call Mapping(MFeatureName, MFile, LFile, MapResult, Sim)

        Next

        'Save new RDF file and open it in IE
        If SaveRDF2.ShowDialog() = DialogResult.OK Then
            Dim myStreamWriter As New StreamWriter(SaveRDF2.FileName)
            If Not (myStreamWriter Is Nothing) Then
                myStreamWriter.Close()
                Dim dTaskID As Double, path As String
                MFile.Save(SaveRDF2.FileName)
                path = "C:\Program Files\Internet Explorer\iexplore.exe"
                dTaskID = Shell(path + " " + SaveRDF2.FileName, vbNormalFocus)
                Me.Close()
                Form1.Visible = True
            End If
        End If

    End Sub

    Public Sub Mapping(ByVal MFeatureName As String, ByRef RDF1File As XDocument, ByRef Lib2File As XDocument, ByRef Mapresult As String, ByRef Sim As Double)
        Dim i As Integer
        Dim j As Integer

        Dim LFeatureName As String
        Dim MAttrCount As Integer
        Dim LAttrCount As Integer

        Dim MFeaName(8) As String
        Dim MNameCount As Integer
        Dim LFeaName(8) As String
        Dim LNameCount As Integer
        Dim MAttrName(10) As String
        Dim LAttrName(10) As String
        Dim AttrNode As XElement
        Dim FeatureAttrName As String

        Dim Count As Integer = 0

        Dim SourceSim As Double
        Dim TargetSim As Double

        Dim BestMap As String
        Dim BestSim As Double = 0
        Dim BestS_Sim As Double = 0
        Dim BestT_Sim As Double = 0
        Dim BestFeatureName As String

        'Feature and Attribute information for the model RDF
        Dim MFeature = From p In RDF1File.Descendants _
                Where p.@rdfs:label = MFeatureName _
                Select p

        Dim MAttrs = From pp In MFeature...<kb:reference> _
                     Select pp.@rdf:resource

        'Artificial attribute
        Dim MaAttrs = From pp2 In MFeature...<kb:a_reference> _
             Select pp2.@rdf:resource

        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim rdf As XNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Dim a_attrname As String
        i = 0
        Dim nref As XElement
        Dim ares As XAttribute

        'Change a_reference into reference. In the mapping process a_reference and reference are same treated
        If MaAttrs.Count <> 0 Then
            Dim ma As XElement = MFeature.First.<kb:a_reference>(0)
            a_attrname = ma.@rdf:resource.ToString
            ma.Remove()
            'ma.ReplaceWith(<kb:reference/>)
            'ma.ReplaceWith(<kb:reference rdf:resource="&amp;kb;Surface_3"/>)
            'a_attrname1 = "<kb:reference rdf:resource= """ & a_attrname & """/>"
            'ma.ReplaceWith(a_attrname1)
            nref = New XElement(kb + "reference")
            ares = New XAttribute(rdf + "resource", a_attrname)
            MFeature.First.Add(nref)
            nref.Add(ares)
            'ma.Add(ares)

        End If


        'Number of all attributes
        MAttrCount = MAttrs.Count + MaAttrs.Count

        'Number of alias
        Dim MAlias = From ppp In MFeature...<kb:Alias> _
                     Select ppp.@rdf:resource
        MNameCount = MAlias.Count + 1

        'First name
        MFeaName(0) = MFeatureName.Split("_")(0) & "_" & MFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_")(2)
        i = 1
        For Each FeatureAlias In MAlias
            MFeaName(i) = FeatureAlias.ToString
            i += 1
        Next

        'Get the name of each attributes in model file
        i = 0
        For Each FeatureAttr In MAttrs
            FeatureAttrName = FeatureAttr.Split(";")(1)
            'Go to the attbiute node, get the name and type of attribute.
            Dim AttrQuery = From p In RDF1File.Descendants _
                Where p.@rdfs:label = FeatureAttrName _
                Select p
            'If an attribute is feature, name it as "Feature_Multi", used for mirror only
            If FeatureAttrName.Contains("Feature") Then
                MAttrName(i) = "Feature_Multi"
                i += 1
            Else
                AttrNode = AttrQuery.First
                'Different format in attributes, sketch and point has no type.
                If FeatureAttrName.Split("_")(0) <> "Sketch" And FeatureAttrName.Split("_")(0) <> "Point" Then
                    MAttrName(i) = FeatureAttrName.Split("_")(0) & "_" & AttrNode.@kb:Type.ToString

                Else : MAttrName(i) = FeatureAttrName.Split("_")(0)
                End If
                i += 1
            End If
        Next

        'Same procedure in artificial attributes, safe artificial attributes as a regular attributes
        For Each FeatureAttr In MaAttrs
            FeatureAttrName = FeatureAttr.Split(";")(1)
            Dim AttrQuery = From p In RDF1File.Descendants _
                Where p.@rdfs:label = FeatureAttrName _
                Select p
            If FeatureAttrName.Contains("Feature") Then
                MAttrName(i) = "Feature_Multi"
                i += 1
            Else
                AttrNode = AttrQuery.First
                If FeatureAttrName.Split("_")(0) <> "Sketch" And FeatureAttrName.Split("_")(0) <> "Point" Then
                    MAttrName(i) = FeatureAttrName.Split("_")(0) & "_" & AttrNode.@kb:Type.ToString

                Else : MAttrName(i) = FeatureAttrName.Split("_")(0)
                End If
                i += 1
            End If
        Next

        'Feature and Attr information for the library RDF
        Dim query2 = From p In Lib2File.Descendants _
            Where p.Name.ToString.Contains("Feature") _
            Select p

        Best_Result.Text = ""

        For Each record2 In query2

            'Get each FeatureName in library.
            LFeatureName = record2.@rdfs:label
            Lib2_Feature.Text = LFeatureName
            Dim LFeature = From p In Lib2File.Descendants _
                Where p.@rdfs:label = LFeatureName _
                Select p
            Dim LAttrs = From pp In LFeature...<kb:reference> _
                         Select pp.@rdf:resource
            LAttrCount = LAttrs.Count

            'Get attributes of target feature
            Dim LAlias = From ppp In LFeature...<kb:Alias> _
                         Select ppp.@rdf:resource
            LNameCount = LAlias.Count + 1

            'Get alias of target feature
            LFeaName(0) = LFeatureName
            i = 1
            For Each FeatureAlias In LAlias
                LFeaName(i) = FeatureAlias.ToString
                i += 1
            Next

            i = 0
            For Each FeatureAttr In LAttrs
                LAttrName(i) = FeatureAttr.Split(";")(1)
                i += 1
            Next

            'Feature mapping
            Mapresult = "Label_UnMatch"
            'If sourse feature name contains target feature name, match, else, unmatch
            For i = 0 To MNameCount - 1
                For j = 0 To LNameCount - 1
                    If MFeaName(i).Contains(LFeaName(j)) Then
                        Mapresult = "Label_Match"

                        'Stop
                    End If
                Next
            Next

            'Count the number of common attributes and show the result in textbox
            Count = 0
            RDF1_Common.Text = ""
            Lib2_Common.Text = ""
            RDF1_Redundant.Text = ""
            Lib2_Missing.Text = ""
            Dim bMAttrName(10) As String
            For i = 0 To MAttrCount - 1
                bMAttrName(i) = MAttrName(i)
                For j = 0 To LAttrCount - 1
                    If MAttrName(i) = LAttrName(j) Then
                        RDF1_Common.Text = RDF1_Common.Text & LAttrName(j) & vbCrLf
                        Lib2_Common.Text = Lib2_Common.Text & LAttrName(j) & vbCrLf
                        MAttrName(i) = ""
                        LAttrName(j) = ""
                        Count += 1
                        Exit For
                    End If

                Next

            Next

            'Show redundant attributes in source feature
            For i = 0 To MAttrCount - 1
                If MAttrName(i) <> "" Then
                    RDF1_Redundant.Text = RDF1_Redundant.Text & MAttrName(i) & vbCrLf
                Else
                    MAttrName(i) = bMAttrName(i)
                End If

            Next

            'Show missing attributes
            For j = 0 To LAttrCount - 1
                If LAttrName(j) <> "" Then
                    Lib2_Missing.Text = Lib2_Missing.Text & LAttrName(j) & vbCrLf
                End If
            Next

            Sim = 0
            SourceSim = 0
            TargetSim = 0
            SourceSim = Count / MAttrCount
            TargetSim = Count / LAttrCount

            If MAttrCount > LAttrCount Then
                Sim = SourceSim
            Else
                Sim = TargetSim
            End If

            Result.Text = ""

            'Show results of four conditions
            If SourceSim = 1 And TargetSim = 1 Then
                Result.Text = "Label match result:" & Mapresult & vbCrLf
                Result.Text = Result.Text & LFeatureName & vbCrLf
                Result.Text = Result.Text & "Source Sim:" & SourceSim & vbCrLf
                Result.Text = Result.Text & "Target Sim:" & TargetSim & vbCrLf
                Result.Text = Result.Text & "Source file and Target file are Equivalent" & vbCrLf

            ElseIf SourceSim < 1 And TargetSim = 1 Then
                Result.Text = "Label match result:" & Mapresult & vbCrLf
                Result.Text = Result.Text & LFeatureName & vbCrLf
                Result.Text = Result.Text & "Source Sim:" & SourceSim & vbCrLf
                Result.Text = Result.Text & "Target Sim:" & TargetSim & vbCrLf
                Result.Text = Result.Text & "Target file is Sub_Equivalent to Source file" & vbCrLf

            ElseIf SourceSim = 1 And TargetSim < 1 Then
                Result.Text = "Label match result:" & Mapresult & vbCrLf
                Result.Text = Result.Text & LFeatureName & vbCrLf
                Result.Text = Result.Text & "Source Sim:" & SourceSim & vbCrLf
                Result.Text = Result.Text & "Target Sim:" & TargetSim & vbCrLf
                Result.Text = Result.Text & "Source file is Sub_Equivalent to Target file" & vbCrLf

            ElseIf SourceSim < 1 And TargetSim < 1 Then
                Result.Text = "Label match result:" & Mapresult & vbCrLf
                Result.Text = Result.Text & LFeatureName & vbCrLf
                Result.Text = Result.Text & "Source Sim:" & SourceSim & vbCrLf
                Result.Text = Result.Text & "Target Sim:" & TargetSim & vbCrLf
                Result.Text = Result.Text & "Source file and Target file are No_Equivalent" & vbCrLf

            End If

            If Sim > BestSim Then
                BestS_Sim = SourceSim
                BestT_Sim = TargetSim
                BestSim = Sim
                BestMap = Mapresult
                BestFeatureName = LFeatureName
                Best_Result.Text = Result.Text
            End If

            'If MFeatureName.Contains("ExtrudedProtrusion_1") Then
            MsgBox("Next Target Feature")
            'End If

        Next

        'Change RDF file, generate RDF2 based on four conditions
        If BestS_Sim = 1 And BestT_Sim = 1 Then
            Call Equt(RDF1File, MFeatureName, BestFeatureName)
        ElseIf BestS_Sim < 1 And BestT_Sim = 1 Then
            Call TSubEqut(RDF1File, Lib2File, MFeatureName, BestFeatureName)
        ElseIf BestS_Sim < 1 And BestT_Sim < 1 Then
            Call SSubEqut(RDF1File, Lib2File, MFeatureName, BestFeatureName)
        ElseIf BestS_Sim < 1 And BestT_Sim < 1 Then
            Call NoEqut(RDF1File, Lib2File, MFeatureName, BestFeatureName)
        End If

    End Sub

    ' Equivalent,change the feature name only
    Sub Equt(ByRef RDF1File As XDocument, ByVal MFeatureName As String, ByVal LFeatureName As String)
        'Change the feature name and the feature attribute node

        Dim MFeature = From p In RDF1File.Descendants _
        Where p.@rdfs:label = MFeatureName _
        Select p

        'Change feature name in feature nodes
        For Each MAtt In MFeature
            MAtt.Name = "Feature_" & LFeatureName.Split("_")(1)
            MAtt.@kb:about = "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
            MAtt.@rdfs:label = "&kb;" & "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
        Next

        'Change the feature name in reference attributes
        Dim AttrFeature = From Attr In RDF1File.Descendants...<kb:reference> _
        Where Attr.@rdf:resource.ToString.Contains("Feature") _
        Select Attr

        For Each element In AttrFeature
            If element.@rdf:resource.Contains(MFeatureName.Split("_")(1)) Then
                element.@rdf:resource = "&kb;Feature_" & LFeatureName.Split("_")(1) & "_" & element.@rdf:resource.Split("_").Last
            End If
        Next
    End Sub

    'Target is subequivalent to source, keep all the target reference, delete redundant reference attributes in source
    Sub TSubEqut(ByRef RDF1File As XDocument, ByRef Lib2File As XDocument, ByVal MFeatureName As String, ByVal LFeatureName As String)
        'Change the feature name and the feature attribute node
        Dim MAttrCount As Integer
        Dim LAttrCount As Integer
        Dim i As Integer
        Dim MAttrName(10) As String
        Dim LAttrName(10) As String
        Dim AttrNode As XElement
        Dim FeatureAttrName As String

        Dim MFeature = From p In RDF1File.Descendants _
        Where p.@rdfs:label = MFeatureName _
        Select p
        Dim MAttrs = From pp In MFeature...<kb:reference> _
             Select pp.@rdf:resource
        MAttrCount = MAttrs.Count

        Dim MaAttrs = From pp2 In MFeature...<kb:a_reference> _
            Select pp2.@rdf:resource
        MAttrCount = MAttrCount + MaAttrs.Count

        'Change feature name in feature nodes
        For Each MAtt In MFeature
            MAtt.Name = "Feature_" & LFeatureName.Split("_")(1)
            MAtt.@kb:about = "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
            MAtt.@rdfs:label = "&kb;" & "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
        Next

        Dim LFeature = From pp In Lib2File.Descendants _
        Where pp.@rdfs:label = LFeatureName _
        Select pp
        Dim LAttrs = From pp In LFeature...<kb:reference> _
             Select pp.@rdf:resource
        LAttrCount = LAttrs.Count

        i = 0
        'Get attribute name in reference
        For Each FeatureAttr In MAttrs
            FeatureAttrName = FeatureAttr.Split(";")(1)
            Dim AttrQuery = From p In RDF1File.Descendants _
                Where p.@rdfs:label = FeatureAttrName _
                Select p
            If FeatureAttrName.Contains("Feature") Then
                MAttrName(i) = FeatureAttrName
                i += 1
            Else
                AttrNode = AttrQuery.First
                If FeatureAttrName.Split("_")(0) <> "Sketch" And FeatureAttrName.Split("_")(0) <> "Point" Then
                    MAttrName(i) = FeatureAttrName.Split("_")(0) & "_" & AttrNode.@kb:Type.ToString

                Else : MAttrName(i) = FeatureAttrName.Split("_")(0)
                End If
                i += 1
            End If
        Next

        'Get attribute name in a_reference
        For Each FeatureAttr In MaAttrs
            FeatureAttrName = FeatureAttr.Split(";")(1)
            Dim AttrQuery = From p In RDF1File.Descendants _
                Where p.@rdfs:label = FeatureAttrName _
                Select p
            If FeatureAttrName.Contains("Feature") Then
                MAttrName(i) = FeatureAttrName
                i += 1
            Else
                AttrNode = AttrQuery.First
                If FeatureAttrName.Split("_")(0) <> "Sketch" And FeatureAttrName.Split("_")(0) <> "Point" Then
                    MAttrName(i) = FeatureAttrName.Split("_")(0) & "_" & AttrNode.@kb:Type.ToString

                Else : MAttrName(i) = FeatureAttrName.Split("_")(0)
                End If
                i += 1
            End If
        Next

        i = 0
        For Each FeatureAttr In LAttrs
            LAttrName(i) = FeatureAttr.Split(";")(1)
            i += 1
        Next

        For Each MAtt In MFeature
            MAtt.Name = "Feature_" & LFeatureName.Split("_")(1)
            MAtt.@kb:about = "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
            MAtt.@rdfs:label = "&kb;" & "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
        Next

        For i = 0 To LAttrCount - 1
            For j = 0 To MAttrCount - 1
                If MAttrName(j) = LAttrName(i) Then
                    MAttrName(j) = ""
                    Exit For
                End If

                'Condition of feature as an attribute
                If MAttrName(j) <> "" Then
                    If LAttrName(i).Contains("Feature_Multi") And MAttrName(j).Contains("Feature") Then
                        MAttrName(j) = ""
                    End If
                End If
            Next

        Next

        'Remove redundant attributes
        For Each FeatureAttr In MAttrs
            If MAttrName(i) <> "" Then
                'AttrNode = MAttrs.
                FeatureAttr.Remove(i)
            End If
            i += 1
        Next

        'Change the feature name if the feature is an attribute
        Dim AttrFeature = From Attr In RDF1File.Descendants...<kb:reference> _
        Where Attr.@rdf:resource.ToString.Contains("Feature") _
        Select Attr

        For Each element In AttrFeature
            If element.@rdf:resource.Contains(MFeatureName.Split("_")(1)) Then
                element.@rdf:resource = "&kb;Feature_" & LFeatureName.Split("_")(1) & "_" & element.@rdf:resource.Split("_").Last
            End If
        Next

    End Sub


    'Source is subequivalent to Target, generate missing attributes
    Sub SSubEqut(ByRef RDF1File As XDocument, ByRef Lib2File As XDocument, ByVal MFeatureName As String, ByVal LFeatureName As String)

        Dim rdf As XNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim rdfs As XNamespace = "http://www.w3.org/2000/01/rdf-schema#"
        Dim nRef As XElement
        Dim aRes As XAttribute
        Dim nRDF As XElement = RDF1File.Root
        'Dim nSurface As XElement
        Dim aAbout As XAttribute
        Dim aLabel As XAttribute

        'Change the feature name and the feature attribute node
        Dim MAttrCount As Integer
        Dim LAttrCount As Integer
        Dim i As Integer
        Dim MAttrName(10) As String
        Dim LAttrName(10) As String
        Dim AttrNode As XElement
        Dim FeatureAttrName As String

        Dim MFeature = From p In RDF1File.Descendants _
        Where p.@rdfs:label = MFeatureName _
        Select p

        Dim nFeature As XElement = MFeature.First

        Dim MAttrs = From pp In MFeature...<kb:reference> _
             Select pp.@rdf:resource
        MAttrCount = MAttrs.Count

        Dim MaAttrs = From pp2 In MFeature...<kb:a_reference> _
             Select pp2.@rdf:resource
        MAttrCount = MAttrCount + MaAttrs.Count

        Dim LFeature = From pp In Lib2File.Descendants _
        Where pp.@rdfs:label = LFeatureName _
        Select pp
        Dim LAttrs = From pp In LFeature...<kb:reference> _
             Select pp.@rdf:resource
        LAttrCount = LAttrs.Count

        i = 0
        For Each FeatureAttr In MAttrs
            FeatureAttrName = FeatureAttr.Split(";")(1)
            Dim AttrQuery = From p In RDF1File.Descendants _
                Where p.@rdfs:label = FeatureAttrName _
                Select p
            If FeatureAttrName.Contains("Feature") Then
                MAttrName(i) = FeatureAttrName
                i += 1
            Else
                AttrNode = AttrQuery.First
                If FeatureAttrName.Split("_")(0) <> "Sketch" And FeatureAttrName.Split("_")(0) <> "Point" Then
                    MAttrName(i) = FeatureAttrName.Split("_")(0) & "_" & AttrNode.@kb:Type.ToString

                Else : MAttrName(i) = FeatureAttrName.Split("_")(0)
                End If
                i += 1
            End If
        Next

        For Each FeatureAttr In MaAttrs
            FeatureAttrName = FeatureAttr.Split(";")(1)
            Dim AttrQuery = From p In RDF1File.Descendants _
                Where p.@rdfs:label = FeatureAttrName _
                Select p
            If FeatureAttrName.Contains("Feature") Then
                MAttrName(i) = FeatureAttrName
                i += 1
            Else
                AttrNode = AttrQuery.First
                If FeatureAttrName.Split("_")(0) <> "Sketch" And FeatureAttrName.Split("_")(0) <> "Point" Then
                    MAttrName(i) = FeatureAttrName.Split("_")(0) & "_" & AttrNode.@kb:Type.ToString

                Else : MAttrName(i) = FeatureAttrName.Split("_")(0)
                End If
                i += 1
            End If
        Next

        i = 0
        For Each FeatureAttr In LAttrs
            LAttrName(i) = FeatureAttr.Split(";")(1)
            i += 1
        Next

        'Change feature name in feature node
        For Each MAtt In MFeature
            MAtt.Name = "Feature_" & LFeatureName.Split("_")(1)
            MAtt.@kb:about = "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
            MAtt.@rdfs:label = "&kb;" & "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
        Next

        'Buffer
        Dim bLAttrName(10) As String
        Dim bMAttrName(10) As String
        'I am trying to use MapIndex to count the useful attribute to generate missing attributes, if point is 1, surface is 0.1
        'then MapIndex = 3 means three points, can generate a plane, if MapIndex = 1.1, means one point and one plane,
        'can generate a new plane parallel to that one. the problem is if MapIndex = 3.1, how to decide.
        'Here is just use three point to generate a plane.
        Dim MapIndex As Integer = 0

        For i = 0 To MAttrCount - 1
            bMAttrName(i) = MAttrName(i)
            If MAttrName(i).Contains("Point") Then
                MapIndex += 1
            End If
        Next
        For i = 0 To LAttrCount - 1
            bLAttrName(i) = LAttrName(i)
            For j = 0 To MAttrCount - 1
                If MAttrName(j) = LAttrName(i) Then
                    MAttrName(j) = ""
                    bLAttrName(i) = ""
                    Exit For
                End If
                If LAttrName(i).Contains("Feature_Multi") And MAttrName(j).Contains("Feature") Then
                    MAttrName(j) = ""
                    Exit For
                End If

            Next
        Next


        For i = 0 To LAttrCount - 1
            If bLAttrName(i).Contains("Surface") Then
                Select Case MapIndex
                    'Create a plane using three points.
                    Case 3
                        Dim X(2) As Double
                        Dim Y(2) As Double
                        Dim Z(2) As Double

                        Dim AttrQuery = From p In RDF1File.Descendants _
                        Where p.Name.ToString.Contains("Point") _
                        Select p
                        Dim j As Integer = 0
                        For Each Attr1 In AttrQuery
                            X(j) = CDbl(Attr1.@kb:X)
                            Y(j) = CDbl(Attr1.@kb:Y)
                            Z(j) = CDbl(Attr1.@kb:Z)
                            j += 1
                        Next


                        nRef = New XElement(kb + "reference")
                        aRes = New XAttribute(rdf + "resource", "&kb;Surface_" & 0)
                        nFeature.Add(nRef)
                        nRef.Add(aRes)

                        nSurface = New XElement(kb + "Surface")
                        aAbout = New XAttribute(rdf + "about", "&kb;Surface_" & 0)
                        aLabel = New XAttribute(rdfs + "label", "Surface_" & 0)
                        nRDF.Add(nSurface)
                        nSurface.Add(aAbout)
                        nSurface.Add(New XAttribute(kb + "Type", "From"))
                        nSurface.Add(New XAttribute(kb + "P1X", X(0)))
                        nSurface.Add(New XAttribute(kb + "P1Y", Y(0)))
                        nSurface.Add(New XAttribute(kb + "P1Z", Z(0)))
                        nSurface.Add(New XAttribute(kb + "P2X", X(1)))
                        nSurface.Add(New XAttribute(kb + "P2Y", Y(1)))
                        nSurface.Add(New XAttribute(kb + "P2Z", Z(1)))
                        nSurface.Add(New XAttribute(kb + "P3X", X(2)))
                        nSurface.Add(New XAttribute(kb + "P3Y", Y(2)))
                        nSurface.Add(New XAttribute(kb + "P3Z", Z(2)))
                        nSurface.Add(aLabel)

                End Select
            End If
        Next

        Dim AttrFeature = From Attr In RDF1File.Descendants...<kb:reference> _
Where Attr.@rdf:resource.ToString.Contains("Feature") _
Select Attr

        For Each element In AttrFeature
            If element.@rdf:resource.Contains(MFeatureName.Split("_")(1)) Then
                element.@rdf:resource = "&kb;Feature_" & LFeatureName.Split("_")(1) & "_" & element.@rdf:resource.Split("_").Last
            End If
        Next

    End Sub

    Sub NoEqut(ByRef RDF1File As XDocument, ByRef Lib2File As XDocument, ByVal MFeatureName As String, ByVal LFeatureName As String)
        
        Dim rdf As XNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Dim kb As XNamespace = "http://www.e-designcenter.info/kb#"
        Dim rdfs As XNamespace = "http://www.w3.org/2000/01/rdf-schema#"
        Dim nRef As XElement
        Dim aRes As XAttribute
        Dim nRDF As XElement = RDF1File.Root
        'Dim nSurface As XElement
        Dim aAbout As XAttribute
        Dim aLabel As XAttribute

        'Change the feature name and the feature attribute node
        Dim MAttrCount As Integer
        Dim LAttrCount As Integer
        Dim i As Integer
        Dim MAttrName(10) As String
        Dim LAttrName(10) As String
        Dim AttrNode As XElement
        Dim FeatureAttrName As String

        Dim MFeature = From p In RDF1File.Descendants _
        Where p.@rdfs:label = MFeatureName _
        Select p

        Dim nFeature As XElement = MFeature.First

        Dim MAttrs = From pp In MFeature...<kb:reference> _
             Select pp.@rdf:resource
        MAttrCount = MAttrs.Count

        Dim MaAttrs = From pp2 In MFeature...<kb:a_reference> _
             Select pp2.@rdf:resource
        MAttrCount = MAttrCount + MaAttrs.Count

        Dim LFeature = From pp In Lib2File.Descendants _
        Where pp.@rdfs:label = LFeatureName _
        Select pp
        Dim LAttrs = From pp In LFeature...<kb:reference> _
             Select pp.@rdf:resource
        LAttrCount = LAttrs.Count

        i = 0
        For Each FeatureAttr In MAttrs
            FeatureAttrName = FeatureAttr.Split(";")(1)
            Dim AttrQuery = From p In RDF1File.Descendants _
                Where p.@rdfs:label = FeatureAttrName _
                Select p
            If FeatureAttrName.Contains("Feature") Then
                MAttrName(i) = FeatureAttrName
                i += 1
            Else
                AttrNode = AttrQuery.First
                If FeatureAttrName.Split("_")(0) <> "Sketch" And FeatureAttrName.Split("_")(0) <> "Point" Then
                    MAttrName(i) = FeatureAttrName.Split("_")(0) & "_" & AttrNode.@kb:Type.ToString

                Else : MAttrName(i) = FeatureAttrName.Split("_")(0)
                End If
                i += 1
            End If
        Next

        For Each FeatureAttr In MaAttrs
            FeatureAttrName = FeatureAttr.Split(";")(1)
            Dim AttrQuery = From p In RDF1File.Descendants _
                Where p.@rdfs:label = FeatureAttrName _
                Select p
            If FeatureAttrName.Contains("Feature") Then
                MAttrName(i) = FeatureAttrName
                i += 1
            Else
                AttrNode = AttrQuery.First
                If FeatureAttrName.Split("_")(0) <> "Sketch" And FeatureAttrName.Split("_")(0) <> "Point" Then
                    MAttrName(i) = FeatureAttrName.Split("_")(0) & "_" & AttrNode.@kb:Type.ToString

                Else : MAttrName(i) = FeatureAttrName.Split("_")(0)
                End If
                i += 1
            End If
        Next

        i = 0
        For Each FeatureAttr In LAttrs
            LAttrName(i) = FeatureAttr.Split(";")(1)
            i += 1
        Next

        'Change feature name in feature node
        For Each MAtt In MFeature
            MAtt.Name = "Feature_" & LFeatureName.Split("_")(1)
            MAtt.@kb:about = "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
            MAtt.@rdfs:label = "&kb;" & "Feature_" & LFeatureName.Split("_")(1) & "_" & MFeatureName.Split("_").Last
        Next

        'Buffer
        Dim bLAttrName(10) As String
        Dim bMAttrName(10) As String
        'I am trying to use MapIndex to count the useful attribute to generate missing attributes, if point is 1, surface is 0.1
        'then MapIndex = 3 means three points, can generate a plane, if MapIndex = 1.1, means one point and one plane,
        'can generate a new plane parallel to that one. the problem is if MapIndex = 3.1, how to decide.
        'Here is just use three point to generate a plane.
        Dim MapIndex As Integer = 0

        For i = 0 To MAttrCount - 1
            bMAttrName(i) = MAttrName(i)
            If MAttrName(i).Contains("Point") Then
                MapIndex += 1
            End If
        Next
        For i = 0 To LAttrCount - 1
            bLAttrName(i) = LAttrName(i)
            For j = 0 To MAttrCount - 1
                If MAttrName(j) = LAttrName(i) Then
                    MAttrName(j) = ""
                    bLAttrName(i) = ""
                    Exit For
                End If
                If LAttrName(i).Contains("Feature_Multi") And MAttrName(j).Contains("Feature") Then
                    MAttrName(j) = ""
                    Exit For
                End If

            Next
        Next


        For i = 0 To LAttrCount - 1
            If bLAttrName(i).Contains("Surface") Then
                Select Case MapIndex
                    'Create a plane using three points.
                    Case 3
                        Dim X(2) As Double
                        Dim Y(2) As Double
                        Dim Z(2) As Double

                        Dim AttrQuery = From p In RDF1File.Descendants _
                        Where p.Name.ToString.Contains("Point") _
                        Select p
                        Dim j As Integer = 0
                        For Each Attr1 In AttrQuery
                            X(j) = CDbl(Attr1.@kb:X)
                            Y(j) = CDbl(Attr1.@kb:Y)
                            Z(j) = CDbl(Attr1.@kb:Z)
                            j += 1
                        Next


                        nRef = New XElement(kb + "reference")
                        aRes = New XAttribute(rdf + "resource", "&kb;Surface_" & 0)
                        nFeature.Add(nRef)
                        nRef.Add(aRes)

                        nSurface = New XElement(kb + "Surface")
                        aAbout = New XAttribute(rdf + "about", "&kb;Surface_" & 0)
                        aLabel = New XAttribute(rdfs + "label", "Surface_" & 0)
                        nRDF.Add(nSurface)
                        nSurface.Add(aAbout)
                        nSurface.Add(New XAttribute(kb + "Type", "From"))
                        nSurface.Add(New XAttribute(kb + "P1X", X(0)))
                        nSurface.Add(New XAttribute(kb + "P1Y", Y(0)))
                        nSurface.Add(New XAttribute(kb + "P1Z", Z(0)))
                        nSurface.Add(New XAttribute(kb + "P2X", X(1)))
                        nSurface.Add(New XAttribute(kb + "P2Y", Y(1)))
                        nSurface.Add(New XAttribute(kb + "P2Z", Z(1)))
                        nSurface.Add(New XAttribute(kb + "P3X", X(2)))
                        nSurface.Add(New XAttribute(kb + "P3Y", Y(2)))
                        nSurface.Add(New XAttribute(kb + "P3Z", Z(2)))
                        nSurface.Add(aLabel)

                End Select
            End If
        Next

        i = 0
        'Remove redundant attributes
        For Each FeatureAttr In MAttrs
            If MAttrName(i) <> "" Then
                'AttrNode = MAttrs.
                FeatureAttr.Remove(i)
            End If
            i += 1
        Next

        Dim AttrFeature = From Attr In RDF1File.Descendants...<kb:reference> _
Where Attr.@rdf:resource.ToString.Contains("Feature") _
Select Attr

        For Each element In AttrFeature
            If element.@rdf:resource.Contains(MFeatureName.Split("_")(1)) Then
                element.@rdf:resource = "&kb;Feature_" & LFeatureName.Split("_")(1) & "_" & element.@rdf:resource.Split("_").Last
            End If
        Next
    End Sub

End Class
