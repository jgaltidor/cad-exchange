﻿Imports System.Xml.XPath
Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst
Imports System.Collections.Generic

Namespace SWImporter

Public Class SWXMLSketchParser
        'Root XML node of input XML file 
	Private nav As XPathNavigator

	'Model to insert sketch to
	Private swModel As IModelDoc2

	'Maps entities ids from the input XML to ids of the generated entities
	Private eidmap As New Dictionary(Of String, Object)


	Public Sub New(ByRef filepath As String, ByRef inputSWModel As IModelDoc2)
		Me.New(New XPathDocument(filepath), inputSWModel)
	End Sub

	Public Sub New(ByRef document As XPathDocument, ByRef inputSWModel As IModelDoc2)
		Dim doc As XPathDocument = document
		nav = doc.CreateNavigator()
		swModel = inputSWModel
	End Sub


	Public Sub insertSketch()
		'insert a sketch is true so now the model can be drawn on the newly created sketch
		swModel.SketchManager.InsertSketch(True)
		'gets the first feature in the part document
		' Dim swFeat As Feature = swModel.FirstFeature
		createEntities()
		createConstraints()
		createDimensions()
		'close the sketch
		swModel.SketchManager.InsertSketch(True)
	End Sub

	Public Function getEntityNodes() As XPathNodeIterator
		Return nav.Select("/sw2DSection/sw2DEntities/sw2DEntity")
	End Function

	Public Function getConstraintNodes() As XPathNodeIterator
		Return nav.Select("/sw2DSection/sw2DConstraints/sw2DConstraint")
	End Function

	Public Function getDimensionNodes() As XPathNodeIterator
		Return nav.Select("/sw2DSection/sw2DDimensions/sw2DDimension")
	End Function

	Public Sub createEntities()
		For Each node As XPathNavigator In getEntityNodes()
			createEntity(node)
		Next
	End Sub


	Public Sub createConstraints()
		For Each node As XPathNavigator In getConstraintNodes()
			createConstraint(node)
		Next
	End Sub


	Public Sub createDimensions()
		For Each node As XPathNavigator In getDimensionNodes()
			createDimension(node)
		Next
	End Sub


	Private Sub createEntity(ByRef node As XPathNavigator)
            Dim typ As String = GetAttr(node, "type")
		Select Case typ
			Case "swSketchLINE"
				createLine(node)
			Case "swSketchARC"
				createArc(node)
			Case "swSketchELLIPSE"
				createEllipse(node)
			Case "swSketchPARABOLA"
				createParabola(node)
			Case "swSketchPOINT"
                    createPoint(node)
                Case "swSketchSpline"
                    createSpline(node)
                Case Else
                    Console.Error.WriteLine("Unknown entity type: " & typ)
            End Select
	End Sub


	'Expects an XML node with the following format:
	'  <sw2DEntity ID="(0,1)" type="swSketchLINE">
	'    <Start>
	'      <sw2DPt ID="(0,1)" x ="0" y ="0" z ="0" />
	'    </Start>
	'    <End>
	'      <sw2DPt ID="(1,2)" x ="100" y ="0" z ="0" />
	'    </End>
	'</sw2DEntity>
	Private Function createLine(ByRef node As XPathNavigator) As SketchSegment
		Dim spNode As XPathNavigator = GetUniqueNode(node.Select("Start/sw2DPt"))
		Dim epNode As XPathNavigator = GetUniqueNode(node.Select("End/sw2DPt"))
		Dim sp As Double() = GetCoord(spNode)
		Dim ep As Double() = GetCoord(epNode)
		Dim id As String = GetEid(node)

		Console.WriteLine("Creating line for entity " + id.ToString())
		Dim sketch As SketchManager = swModel.SketchManager
		Dim line As ISketchLine = sketch.CreateLine(sp(0), sp(1), sp(2), ep(0), ep(1), ep(2))
		'Add id mappings for entities created
		addEidMapping(id, line)
		addEidMapping(GetEid(spNode), line.IGetStartPoint2())
		addEidMapping(GetEid(epNode), line.IGetEndPoint2())
		Return line
	End Function


	'Expects an XML node with the following format:
	'  <sw2DEntity ID="(0,2)" type="swSketchARC" direction="1" >
	'    <Center>
	'      <sw2DPt ID="(3,12)" x ="50" y ="60" z ="0" />
	'    </Center>
	'    <Start>
	'      <sw2DPt ID="(6,10)" x ="70" y ="60" z ="0" />
	'    </Start>
	'    <End>
	'      <sw2DPt ID="(7,11)" x ="30" y ="60" z ="0" />
	'    </End>
	'  </sw2DEntity>
	Private Function createArc(ByRef node As XPathNavigator) As SketchSegment
		Dim cpNode As XPathNavigator = GetUniqueNode(node.Select("Center/sw2DPt"))
		Dim spNode As XPathNavigator = GetUniqueNode(node.Select("Start/sw2DPt"))
		Dim epNode As XPathNavigator = GetUniqueNode(node.Select("End/sw2DPt"))
		Dim cp As Double() = GetCoord(cpNode)
		Dim sp As Double() = GetCoord(spNode)
		Dim ep As Double() = GetCoord(epNode)
		Dim direction As Short = Short.Parse(GetAttr(node, "direction"))
		Dim id As String = GetEid(node)

		Console.WriteLine("Creating arc for entity " + id.ToString())
		Dim sketch As SketchManager = swModel.SketchManager
		Dim seg As SketchSegment = sketch.CreateArc( _
			cp(0), cp(1), cp(2), sp(0), sp(1), sp(2), ep(0), ep(1), ep(2), direction)
		'Add id mappings for entities created
		addEidMapping(id, seg)
		addEidMapping(GetEid(cpNode), seg.getCenterPoint2)
		addEidMapping(GetEid(spNode), seg.getStartPoint2)
		addEidMapping(GetEid(epNode), seg.getEndPoint2)
		Return seg
	End Function


	'Expects an XML node with the following format:
	'  <sw2DEntity ID="(0,2)" type="swSketchELLIPSE">
	'    <Center>
	'      <sw2DPt ID="(7,11)" x ="30" y ="60" z ="0" />
	'    </Center>
	'    <Major>
	'      <sw2DPt ID="(6,10)" x ="70" y ="60" z ="0" />
	'    </Major>
	'    <Minor>
	'      <sw2DPt ID="(3,12)" x ="50" y ="60" z ="0" />
	'    </Minor>
	'  </sw2DEntity>
	Private Function createEllipse(ByRef node As XPathNavigator) As SketchSegment
		Dim cpNode As XPathNavigator = GetUniqueNode(node.Select("Center/sw2DPt"))
		Dim apNode As XPathNavigator = GetUniqueNode(node.Select("Major/sw2DPt"))
		Dim ipNode As XPathNavigator = GetUniqueNode(node.Select("Minor/sw2DPt"))
		Dim cp As Double() = GetCoord(cpNode)
		Dim ap As Double() = GetCoord(apNode)
		Dim ip As Double() = GetCoord(ipNode)
		Dim id As String = GetEid(node)

		Console.WriteLine("Creating ellipse for entity " + id.ToString())
		Dim sketch As SketchManager = swModel.SketchManager
		Dim seg As SketchSegment = sketch.CreateEllipse( _
			cp(0), cp(1), cp(2), ap(0), ap(1), ap(2), ip(0), ip(1), ip(2))
		'Add id mappings for entities created
		addEidMapping(id, seg)
		addEidMapping(GetEid(cpNode), seg.getCenterPoint2)
		addEidMapping(GetEid(apNode), seg.getMajorPoint2)
		addEidMapping(GetEid(ipNode), seg.getMinorPoint2)
		Return seg
	End Function


	'Expects an XML node with the following format:
	'  <sw2DEntity ID="(0,2)" type="swSketchPARABOLA" >
	'    <Focus>
	'      <sw2DPt ID="(3,12)" x ="50" y ="60" z ="0" />
	'    </Focus>
	'    <Apex>
	'      <sw2DPt ID="(6,10)" x ="70" y ="60" z ="0" />
	'    </Apex>
	'    <Start>
	'      <sw2DPt ID="(8,10)" x ="70" y ="60" z ="0" />
	'    </Start>
	'    <End>
	'      <sw2DPt ID="(7,11)" x ="30" y ="60" z ="0" />
	'    </End>
	'  </sw2DEntity>
	Private Function createParabola(ByRef node As XPathNavigator) As SketchSegment
		Dim fpNode As XPathNavigator = GetUniqueNode(node.Select("Focus/sw2DPt"))
		Dim apNode As XPathNavigator = GetUniqueNode(node.Select("Apex/sw2DPt"))
		Dim spNode As XPathNavigator = GetUniqueNode(node.Select("Start/sw2DPt"))
		Dim epNode As XPathNavigator = GetUniqueNode(node.Select("End/sw2DPt"))
		Dim fp As Double() = GetCoord(fpNode)
		Dim ap As Double() = GetCoord(apNode)
		Dim sp As Double() = GetCoord(spNode)
		Dim ep As Double() = GetCoord(epNode)
		Dim id As String = GetEid(node)

		Console.WriteLine("Creating parabola for entity " + id.ToString())
		Dim sketch As SketchManager = swModel.SketchManager
		Dim seg As SketchSegment = sketch.CreateParabola(fp(0), fp(1), fp(2), _
			ap(0), ap(1), ap(2), sp(0), sp(1), sp(2), ep(0), ep(1), ep(2))
		'Add id mappings for entities created
		addEidMapping(id, seg)
		addEidMapping(GetEid(fpNode), seg.getFocusPoint2)
		addEidMapping(GetEid(apNode), seg.getApexPoint2)
		addEidMapping(GetEid(spNode), seg.getStartPoint2)
		addEidMapping(GetEid(epNode), seg.getEndPoint2)
		Return seg
	End Function


	'Expects an XML node with the following format:
	'  <sw2DEntity ID="(0,1)" type="swSketchPOINT">
	'    <Start>
	'      <sw2DPt ID="(0,1)" x ="0" y ="0" z ="0" />
	'    </Start>
	'  </sw2DEntity>
        Private Function createPoint(ByRef node As XPathNavigator) As SketchPoint
            'Console.WriteLine(node)

            Dim spNode As XPathNavigator = GetUniqueNode(node.Select("Start/sw2DPt"))
            Dim sp As Double() = GetCoord(spNode)
            Dim id As String = GetEid(node)
            Console.WriteLine("Creating point for entity " + id.ToString())
            Dim sketch As SketchManager = swModel.SketchManager
            Dim pt As ISketchPoint = sketch.CreatePoint(sp(0), sp(1), sp(2))
            If pt Is Nothing Then
                'Search if there is already an existing point at the same location
                Dim pair As KeyValuePair(Of String, ISketchPoint) = SearchForPoint(sp(0), sp(1), sp(2), eidmap)
                If pair.Value Is Nothing Then
                    Throw New Exception(String.Format("Cannot create new point ({0}, {1}, {2})", _
                     sp(0), sp(1), sp(2)))
                End If
                pt = pair.Value
            End If
            addEidMapping(id, pt)
            Return pt
        End Function

        '--------------------------------------------------------------------------------
        Private Function createSpline(ByRef node As XPathNavigator) As SketchSegment

            Dim i As Integer
            i = 0
            For Each refnode As XPathNavigator In node.Select("PointArray/sw2DPt")
                i = i + 1
            Next

            Dim points(i) As SketchPoint

            Dim j As Integer
            j = 0

            For Each refnode As XPathNavigator In node.Select("PointArray/sw2DPt")
                Console.WriteLine(j)
                points(j) = createPoint(refnode) 'this line is the problem.
                j = j + 1
            Next

            Console.WriteLine("past 2 for loop")

            Dim sketch As SketchManager = swModel.SketchManager
            Dim spline As ISketchPoint = sketch.CreateSpline(points)

            Dim id As String = GetEid(node)
            Console.WriteLine("Creating spline for entity " + id.ToString())

            Return spline
            '----------------------------------------------------------------------------------------------
        End Function


	'Expects an XML node with the following format:
	'  <sw2DConstraint type="swConstraintType_COINCIDENT">
	'    <entityRefs>
	'      <entityRef ID="(7,2)" />
	'      <entityRef ID="(4,1)" />
	'    </entityRefs>
	'  </sw2DConstraint>

	Private Sub createConstraint(ByRef node As XPathNavigator)
		Dim typ As String = GetAttr(node, "type")
		swModel.ClearSelection2(True) 'Clear selection
		'First, select each entities referred to by the constraint
		For Each refnode As XPathNavigator In node.Select("entityRefs/entityRef")
			SelectEnt(eidmap(GetEid(refnode)))
		Next
		'Add constraint to all selected entities
		swModel.SketchAddConstraints(typ)
		swModel.ClearSelection2(True) 'Clear selection
	End Sub


	Private Sub createDimension(ByRef node As XPathNavigator)
		Dim typ As String = GetAttr(node, "type")
		Dim dimValue As Double = Double.Parse(GetAttr(node, "Value"))
		swModel.ClearSelection2(True) 'Clear selection
		'First, select each entities referred to by the constraint
		Dim firstEnt As Object = Nothing
		For Each refnode As XPathNavigator In node.Select("entityRefs/entityRef")
			Dim entity As Object = eidmap(GetEid(refnode))
			SelectEnt(entity)
			If firstEnt Is Nothing Then
				firstEnt = entity
			End If
		Next
		Dim displaydim As IDisplayDimension = Nothing
		If firstEnt IsNot Nothing Then
			Dim c As Double() = GetCoords(firstEnt)
			Dim offset As Double = 10
			c(0) += offset : c(1) += offset : c(2) += offset
			Select Case typ
				Case "swLinearDimension"
					displaydim = createLinearDimension(node, c(0), c(1), c(2))
				Case "swDiameterDimension"
					displaydim = createDiameterDimension(node, c(0), c(1), c(2))
				Case "swHorLinearDimension"
					displaydim = createHorizontalDimension(node, c(0), c(1), c(2))
				Case "swRadialDimension"
					displaydim = createRadialDimension(node, c(0), c(1), c(2))
				Case "swVertLinearDimension"
					displaydim = createVerticalDimension(node, c(0), c(1), c(2))
				Case Else
					Console.Error.WriteLine("Unknown dimension type: " & typ)
			End Select
		Else
			Select Case typ
				Case "swLinearDimension"
					displaydim = createLinearDimension(node)
				Case "swDiameterDimension"
					displaydim = createDiameterDimension(node)
				Case "swHorLinearDimension"
					displaydim = createHorizontalDimension(node)
				Case "swRadialDimension"
					displaydim = createRadialDimension(node)
				Case "swVertLinearDimension"
					displaydim = createVerticalDimension(node)
				Case Else
					Console.Error.WriteLine("Unknown dimension type: " & typ)
			End Select
		End If
		If displaydim IsNot Nothing Then
			'displaydim.setUnits(False, 2, 1, Nothing, False)
			'Dim dimension As Object = swModel.Parameter(displaydim.GetNameForSelection())
			'dimension.SystemValue = val
			Dim dimension As IDimension = displaydim.GetDimension2(0)
			dimension.SetValue3(dimValue, _
				swInConfigurationOpts_e.swThisConfiguration, _
				Nothing)
		End If
		swModel.ClearSelection2(True) 'Clear selection
	End Sub

	Private Function createLinearDimension(ByRef node As XPathNavigator) As DisplayDimension
		Return createLinearDimension(node, 20, 20, 0)
	End Function

	Private Function createDiameterDimension(ByRef node As XPathNavigator) As DisplayDimension
		Return createDiameterDimension(node, 35, 35, 0)
	End Function

	Private Function createHorizontalDimension(ByRef node As XPathNavigator) As DisplayDimension
		Return createHorizontalDimension(node, 50, 50, 0)
	End Function

	Private Function createRadialDimension(ByRef node As XPathNavigator) As DisplayDimension
		Return createRadialDimension(node, 65, 65, 0)
	End Function

	Private Function createVerticalDimension(ByRef node As XPathNavigator) As DisplayDimension
		Return createVerticalDimension(node, 80, 80, 0)
	End Function


	'x, y, z are coordinates of where the parameter text is shown
	Private Function createLinearDimension(ByRef node As XPathNavigator, ByVal x As Double, _
		ByVal y As Double, ByVal z As Double) As DisplayDimension
		Return swModel.IAddDimension2(x, y, z)
	End Function

	'x, y, z are coordinates of where the parameter text is shown
	Private Function createDiameterDimension(ByRef node As XPathNavigator, ByVal x As Double, _
		ByVal y As Double, ByVal z As Double) As DisplayDimension
		Return swModel.IAddDiameterDimension2(x, y, z)
	End Function

	'x, y, z are coordinates of where the parameter text is shown
	Private Function createHorizontalDimension(ByRef node As XPathNavigator, ByVal x As Double, _
		ByVal y As Double, ByVal z As Double) As DisplayDimension
		Return swModel.IAddHorizontalDimension2(x, y, z)
	End Function

	'x, y, z are coordinates of where the parameter text is shown
	Private Function createRadialDimension(ByRef node As XPathNavigator, ByVal x As Double, _
		ByVal y As Double, ByVal z As Double) As DisplayDimension
		Return swModel.IAddRadialDimension2(x, y, z)
	End Function

	'x, y, z are coordinates of where the parameter text is shown
	Private Function createVerticalDimension(ByRef node As XPathNavigator, ByVal x As Double, _
		ByVal y As Double, ByVal z As Double) As DisplayDimension
		Return swModel.IAddVerticalDimension2(x, y, z)
	End Function


	Private Sub addEidMapping(ByRef id As String, ByRef val As Object)
		AddMapping(eidmap, id, val)
	End Sub

	'Shared Functions

	'Returns the single node element in this iterator
	'Throws an exception if there are no nodes in this iterator
	'or multiple nodes in the iterator
	Public Shared Function GetUniqueNode(ByRef itr As XPathNodeIterator) As XPathNavigator
		If itr.MoveNext() Then
			Dim nav As XPathNavigator = itr.Current
			If itr.MoveNext() Then
				Throw New ArgumentException("Single node expected where multiple nodes found")
			Else
				Return nav
			End If
		Else
			Throw New ArgumentException("Single node expected where no nodes found")
		End If
	End Function


	'node.Current should be an XML node.
	Public Shared Function GetAttr(ByRef node As XPathNavigator, ByRef name As String) As String
		Dim val As String = node.GetAttribute(name, "")
		If val = String.Empty Then
			Throw New Exception(String.Format( _
				"Empty String returned for attribute {0} for XML node {1}", _
				name, node.Name))
		End If
		Return val
	End Function


	'Expects an XML node w/ an ID attribute:
	' Example <elem ID="(4,1)" />
	Public Shared Function GetEid(ByRef node As XPathNavigator) As String
		Return GetAttr(node, "ID")
	End Function


	Public Shared Sub AddMapping(Of TKey, TVal)(ByRef map As IDictionary(Of TKey, TVal), _
		ByRef key As TKey, ByRef val As TVal)
		Try
			map.Add(key, val)
		Catch exc As ArgumentException
			If Not map(key).Equals(val) Then
				Throw New ArgumentException(String.Format("Mapping existing key {0} to new value {1}", _
					key, val))
			End If
		End Try
	End Sub

	Public Shared Function SearchForPoint(ByVal x As Double, ByVal y As Double, ByVal z As Double, _
		ByRef map As IDictionary(Of String, Object)) As KeyValuePair(Of String, ISketchPoint)
		For Each pair In map
			If TypeOf pair.Value Is ISketchPoint Then
				Dim pt As ISketchPoint = pair.Value
					If pt.X = x And pt.Y = y And pt.Z = z Then
						Return New KeyValuePair(Of String, ISketchPoint)(pair.Key, pt)
					End If
			End If
		Next
		Return New KeyValuePair(Of String, ISketchPoint)(String.Empty, Nothing)
	End Function

	Public Shared Function SelectEnt(ByRef selectable As Object) As Boolean
		If TypeOf selectable Is ISketchPoint Then
			Return DirectCast(selectable, ISketchPoint).Select4(True, Nothing)
		ElseIf TypeOf selectable Is ISketchSegment Then
			Return DirectCast(selectable, ISketchSegment).Select4(True, Nothing)
		ElseIf TypeOf selectable Is ISketchHatch Then
			Return DirectCast(selectable, ISketchHatch).Select4(True, Nothing)
		ElseIf TypeOf selectable Is IEntity Then
			Return DirectCast(selectable, IEntity).Select4(True, Nothing)
		Else
			Throw New ArgumentException("Unrecognized type of selectable argument: " & _
				selectable.GetType().ToString())
		End If
	End Function

	'Expects an XML node w/ attributes x, y, and z set to double values:
	' Example: <elem x ="0.0" y ="0.0" z ="0.0" />
	Public Shared Function GetCoord(ByRef node As XPathNavigator) As Double()
		Dim coord(2) As Double
		coord(0) = Double.Parse(GetAttr(node, "x"))
		coord(1) = Double.Parse(GetAttr(node, "y"))
		coord(2) = Double.Parse(GetAttr(node, "z"))
		Return coord
	End Function


	Public Shared Function GetCoords(ByRef ent As Object) As Double()
		If TypeOf ent Is ISketchPoint Then
			Return GetCoords(DirectCast(ent, ISketchPoint))
		ElseIf TypeOf ent Is ISketchLine Then
			Return GetCoords(DirectCast(ent, ISketchLine).IGetEndPoint2())
		Else
			Return Nothing
		End If
	End Function

	Public Shared Function GetCoords(ByRef pnt As ISketchPoint) As Double()
		Dim coord(2) As Double
		coord(0) = pnt.X
		coord(1) = pnt.Y
		coord(2) = pnt.Z
		Return coord
	End Function
End Class

End Namespace 'SWImporter
