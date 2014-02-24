package psconv
import psconv._
import pxmlparser._
import pro2dtosw._
import java.io.File
import scala.collection.JavaConversions._

object MainApp
{
	def convertSection(pfile:File):Unit = {
		println("Converting section file: " + pfile)
		val parser = new ProeXMLParser(pfile) with CachedNode2ProeFactory
		val prosec = parser.createSection
		val (prosubsec, swsection) = new DefaultConversion(prosec).convSection
		val remainingpsec = prosec -- prosubsec
		if(prosubsec.isEmpty) {
			println("No conversion able to be performed")
			println(remainingpsec)
		}
		else if(!remainingpsec.isEmpty) {
			println("Not able to convert remaining Pro/E section:")
			println(remainingpsec)
		}
		else {
			println("Successfully converted entire Pro/E section")
		}
		if(!swsection.isEmpty) {
			println
			val filename = "Sketch_" + swsection.name + ".xml"
			val dirpath = outputDirPath(pfile)
			new File(dirpath).mkdirs
			val sfilepath = dirpath + File.separator + filename
			println("Writing output SW section to file: " + sfilepath)
			val out = new util.TabPrintStream(sfilepath, "  ")
			sxmlgen.SWXMLGenerator.writeSection(out, swsection)
			out.close
			println("Completed writing file: " + sfilepath)
		}
	}
	
	def outputDirPath(pfile:File):String = {
		val pardirpath = pfile.getParent
		if(pardirpath != null) {
			val pardir = new File(pardirpath)
			val outbasename = "sxml_" + pardir.getName
			val grandpardir = pardir.getParent
			if(grandpardir != null)
				grandpardir + outbasename
			else
				outbasename
		}
		else ""
	}

	private val sectionPat = java.util.regex.Pattern.compile("section_.*\\.xml")
	
	def collectSectionFiles(files:Iterable[File],
		secfilepaths:java.util.List[File]):Unit =
	{
		def getFiles(f:File):Unit = {
			if(f.isDirectory)
				f.listFiles.foreach(getFiles)
			else if(f.isFile && sectionPat.matcher(f.getName).matches)
				secfilepaths add f
		}

		for(file <- files) {
			if(file.isDirectory)
				getFiles(file)
			else if(file.isFile)
				secfilepaths add file
		}
	}
	
	def main(args:Array[String]):Unit = {
		if(args.length == 0) {
			Console.err.println("usage: psconv <pro section XML file/dir 1> ... <pro section XML file/dir N>")
			exit(1)
		}
		val sectionFiles = new java.util.LinkedList[File]
		val argFiles = args.map(new File(_))
		collectSectionFiles(argFiles, sectionFiles)
		sectionFiles.foreach(convertSection)
	}
}
