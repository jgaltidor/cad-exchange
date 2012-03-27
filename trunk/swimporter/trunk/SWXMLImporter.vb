Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst
Imports System.Collections.Generic
Imports System.IO

Namespace SWImporter

Module SWXMLImporter

	Sub Main(ByVal args() As String)
		If args.Length < 2 Then
			System.Console.Error.WriteLine("usage: SWXMLImporter <output file> <xml file/dir 1> ... <xml file/dir N>")
			System.Environment.Exit(1)
		End If

		'Get SolidWorks app and set intial settings
		Dim swApp As ISldWorks = GetSWApp()
		'turns off the display box the pops up asking for a dimension so that it can be programmed
		Dim DimValuePreset As Boolean
		DimValuePreset = swApp.GetUserPreferenceToggle(swUserPreferenceToggle_e.swInputDimValOnCreate)
		swApp.SetUserPreferenceToggle(10, False)

		'Create new part model
		Dim swModel As IModelDoc2 = CreateNewSWPart()
		Dim outfilename As String = args(0)

		'Get input XML files
		Dim inputFiles As New List(Of String)
		For i As Integer = 1 To args.Length - 1
			GetFilePaths(args(i), inputFiles)
		Next
		'Read input files and import information into model
		For Each inputFile In inputFiles
			InsertSketch(inputFile, swModel)
		Next
		'Save the part model to the output model file name
		SavePartFile(outfilename, swModel)

		'turns the display box pop ups for dimensions back to its original setting
		swApp.SetUserPreferenceToggle(swUserPreferenceToggle_e.swInputDimValOnCreate, DimValuePreset)
		swApp.ExitApp()	'close SolidWorks
		Console.WriteLine("Program completed successfully")
	End Sub


	Private swApp As ISldWorks = Nothing
	'A method to open solidWorks
	Public Function GetSWApp() As ISldWorks
		If swApp Is Nothing Then
			Console.WriteLine("Launching SolidWorks...")
			swApp = New SldWorks()
			swApp.Visible = True
		End If
		Return swApp
	End Function


	'creates a new part that uses meters as its units
	Public Function CreateNewSWPart() As IModelDoc2
		Dim swApp As ISldWorks = GetSWApp()
		Dim swModel As IModelDoc2 = swApp.NewPart()
		swModel.LengthUnit = 2
		Return swModel
	End Function


	Public Sub InsertSketch(ByRef filepath As String, ByRef swModel As IModelDoc2)
		Console.WriteLine("Drawing Sketch from XML File: {0}", filepath)
		Dim parser As New SWXMLSketchParser(filepath, swModel)
		parser.insertSketch()
		Console.WriteLine("Finished Sketch from XML File: {0}", filepath)
	End Sub

	Public Sub AddXMLFiles(ByRef paths As IEnumerable(Of String), ByRef filepaths As ICollection(Of String))
		For Each path As String In paths
			GetFilePaths(path, filepaths)
		Next
	End Sub


	Public Sub GetFilePaths(ByRef path As String, ByRef filepaths As ICollection(Of String))
		If Directory.Exists(path) Then
			For Each childFile As String In Directory.GetFiles(path, "Sketch*.xml")
				GetFilePaths(childFile, filepaths)
			Next
			For Each childDir As String In Directory.GetDirectories(path)
				GetFilePaths(childDir, filepaths)
			Next
		ElseIf File.Exists(path) And path.EndsWith(".xml") Then
			filepaths.Add(path)
		End If
	End Sub


	Public Sub SavePartFile(ByVal outfilename As String, ByRef swModel As IModelDoc2)
		Dim errors As Integer
		Dim warnings As Integer
		If Not outfilename.EndsWith(".sldprt") Then
			outfilename = outfilename & ".sldprt"
		End If
		outfilename = ToRootedPath(outfilename)
		swModel.Extension.SaveAs(outfilename, _
			swSaveAsVersion_e.swSaveAsCurrentVersion, _
			swSaveAsOptions_e.swSaveAsOptions_Silent, _
			Nothing, errors, warnings)
	End Sub

	Public Function ToRootedPath(ByRef filepath As String) As String
		If Path.IsPathRooted(filepath) Then
			Return filepath
		Else
			'Prepend rooted path of the current working directory
			Return Directory.GetCurrentDirectory() & Path.DirectorySeparatorChar & filepath
		End If
	End Function

End Module


Module Tester
	Sub Main()
		SWXMLImporter.Main(New String() {"..\..\sec_S2D0001.sldprt", "..\..\unittests\Sketch_swsec_S2D0001.xml"})
	End Sub
End Module

End Namespace 'SWImporter
