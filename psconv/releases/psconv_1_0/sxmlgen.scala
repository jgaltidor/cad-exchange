package psconv.sxmlgen
import java.io.File
import psconv.sw._
import util.TabPrintStream

class SWXMLGenerator(output:TabPrintStream) extends util.TabPrintWrapper
{
	import SWXMLGenerator._

	def this(file:File) = this(new TabPrintStream(file))
	
	// set the out stream field to the output parameter 
	out = output
	
	def writeSection(swsec:SW2D):Unit = {
		println("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>")
		printf("<sw2DSection name=\"%s\">", swsec.name)
		println
		incrementTabs
		writeEntities(swsec.entities)
		decrementTabs
		incrementTabs
		writeConstraints(swsec.constraints)
		decrementTabs
		incrementTabs
		writeDimensions(swsec.dimensions)
		decrementTabs
		println("</sw2DSection>")
	}
	
	def writeEntities(entities:List[Entity]):Unit = {
		println("<sw2DEntities>")
		incrementTabs
		entities foreach writeEntity 
		decrementTabs
		println("</sw2DEntities>")
	}

	def writeConstraints(constraints:List[Constraint]):Unit = {
		println("<sw2DConstraints>")
		incrementTabs
		constraints foreach writeConstraint
		decrementTabs
		println("</sw2DConstraints>")
	}

	def writeDimensions(dimensions:List[Dimension]):Unit = {
		println("<sw2DDimensions>")
		incrementTabs
		dimensions foreach writeDimension 
		decrementTabs
		println("</sw2DDimensions>")
	}

	def writeEntity(ent:Entity):Unit = {
		printf("<sw2DEntity ID=\"%s\" type=\"%s\">", ent.id, typstr(ent))
		println
		incrementTabs
		ent match {
			case e:Point => writePointEntity(e)
			case e:Line => writeLine(e)
		}
		decrementTabs
		println("</sw2DEntity>")
	}
	
	def writePointEntity(pnt:Point):Unit = {
		println("<Start>")
		incrementTabs
		writePoint(pnt)
		decrementTabs
		println("</Start>")
	}
	
	def writeLine(line:Line):Unit = {
		println("<Start>")
		incrementTabs
		writePoint(line.start)
		decrementTabs
		println("</Start>")
		println("<End>")
		incrementTabs
		writePoint(line.end)
		decrementTabs
		println("</End>")
	}
	
	def writePoint(pnt:Point):Unit = {
		printf("<sw2DPt ID=\"%s\" x =\"%s\" y =\"%s\" z =\"%s\" />",
			pnt.id, double2Double(pnt.x), double2Double(pnt.y),
			double2Double(pnt.z))
		println
	}

	def writeConstraint(con:Constraint):Unit = {
		printf("<sw2DConstraint type=\"%s\">", typstr(con))
		println
		incrementTabs
		writeEntityRefs(entityRefs(con))
		decrementTabs
		println("</sw2DConstraint>")
	}

	def writeDimension(dim:Dimension):Unit = {
		printf("<sw2DDimension ID=\"%s\" type=\"%s\" Value=\"%s\">",
			dim.id, typstr(dim), double2Double(dim.value))
		println
		incrementTabs
		writeEntityRefs(entityRefs(dim))
		decrementTabs
		println("</sw2DDimension>")
	}

	def writeEntityRefs(entityRefs:List[EntityID]):Unit = {
		println("<entityRefs>")
		incrementTabs
		entityRefs foreach writeEntityRef
		decrementTabs
		println("</entityRefs>")
	}

	def writeEntityRef(eref:EntityID):Unit = {
		printf("<entityRef ID=\"%s\" />", eref)
		println
	}
}

object SWXMLGenerator
{
	type EntityID = (Int, Int)

	def writeSection(out:TabPrintStream, section:SW2D):Unit =
		new SWXMLGenerator(out).writeSection(section)

	def typstr(ent:Entity) = ent match {
		case e:Point => "swSketchPOINT"
		case e:Line => "swSketchLINE"
	}
	
	def typstr(con:Constraint) = con match {
		case c:Coincident => "swConstraintType_COINCIDENT"
		case c:HorizontalConstraint => "swConstraintType_HORIZONTAL"
		case c:VerticalConstraint => "swConstraintType_VERTICAL"
		case c:DistanceConstraint => "swConstraintType_DISTANCE"
	}

	def typstr(dim:Dimension) = dim match {
		case d:LineDim => "swLinearDimension"
		case d:DiameterDim => "swDiameterDimension"
		case d:HorizontalLineDim => "swHorLinearDimension"
		case d:RadialDim => "swRadialDimension"
		case d:VerticalLineDim => "swVertLinearDimension"
	}
	
	def entityRefs(con:Constraint):List[EntityID] = con match {
		case Coincident(point1, point2) =>
			List(point1.id, point2.id)
		case HorizontalConstraint(line) => List(line.id)
		case VerticalConstraint(line) => List(line.id)
		case DistanceConstraint(point1, point2) =>
			List(point1.id, point2.id)
	}

	def entityRefs(dim:Dimension):List[EntityID] = dim match {
		case LineDim(_, _, ent1, ent2) =>
			List(ent1.id, ent2.id)
		case DiameterDim(_, _, ent1, ent2) =>
			List(ent1.id, ent2.id)
		case HorizontalLineDim(_, _, ent1, ent2) =>
			List(ent1.id, ent2.id)
		case RadialDim(_, _, ent1, ent2) =>
			List(ent1.id, ent2.id)
		case VerticalLineDim(_, _, ent1, ent2) =>
			List(ent1.id, ent2.id)
	}
}


object Tester
{
	import pxmlparser._
	import pro2dtosw._

	def main(args:Array[String]):Unit = {
		if(args.length == 0) {
			Console.err.println("usage: scala psconv.sxmlgen.Tester <Pro/E section XML file>")
			exit(1)
		}
		val parser = new ProeXMLParser(args(0)) with CachedNode2ProeFactory
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
			
			val filename = "swsec_" + swsection.name + ".xml"
			println("Writing output SW section to file: " + filename)
			val out = new TabPrintStream(filename, "  ")
			SWXMLGenerator.writeSection(out, swsection)
			out.close
			println("Completed writing file: " + filename)
		}
	}
}

