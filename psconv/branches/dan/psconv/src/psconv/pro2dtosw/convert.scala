package psconv.pro2dtosw
import psconv._
import proe.Pro2D
import sw.SW2D

trait OneToOneConv
{
	// one-to-one conversion
	def optconvert(entity:proe.Entity):Option[sw.Entity] =
		try { Some(conv(entity)) }
		catch { case exc:UnMappedElementException => None }

	def optconvert(constraint:proe.Constraint):Option[sw.Constraint] =
		try { Some(conv(constraint)) }
		catch { case exc:UnMappedElementException => None }

	def optconvert(dimension:proe.Dimension):Option[sw.Dimension] =
		try { Some(conv(dimension)) }
		catch { case exc:UnMappedElementException => None }


	@throws(classOf[UnMappedElementException])
	def conv(entity:proe.Entity):sw.Entity =
		entity match {
			case e:proe.Line => convLine(e)
			case e:proe.PointEntity => convPointEntity(e)
			case e:proe.Spline => convSpline(e)
		}

	@throws(classOf[UnMappedElementException])
	def conv(constraint:proe.Constraint):sw.Constraint =
		constraint match {
			case c:proe.SamePoint => convSamePoint(c)
			case c:proe.HorizontalConstraint => convHorizontalConstraint(c)
			case c:proe.VerticalConstraint => convVerticalConstraint(c)
			case c:proe.PntOnEnt => throw new UnMappedElementException(c)
		}

	@throws(classOf[UnMappedElementException])
	def conv(dimension:proe.Dimension):sw.Dimension =
		dimension match {
			case d:proe.LineDim => convLineDim(d)
			case d:proe.LinePointDim => convLinePointDim(d)
		}

	// Abstract Methods

	// converting entities

	def convLine(line:proe.Line):sw.Line
	
	//-----------------------------------------------------------------------------
	def convSpline(spline:proe.Spline):sw.Spline
	//-----------------------------------------------------------------------------

	def convPointEntity(epnt:proe.PointEntity):sw.Point

	// converting non-entity point
	def convPoint(pnt:proe.Point):sw.Point


	// converting constraints
	def convSamePoint(con:proe.SamePoint):sw.Coincident

	def convHorizontalConstraint(con:proe.HorizontalConstraint):sw.HorizontalConstraint

	def convVerticalConstraint(con:proe.VerticalConstraint):sw.VerticalConstraint


	// converting dimensions

	def convLineDim(dim:proe.LineDim):sw.LineDim
	
	def convLinePointDim(dim:proe.LinePointDim):sw.LineDim
}


/** Represents a conversion function that converts a
  * Pro/E subsection to a SolidWorks subsection.
  * Let optconv be an instance of Converter.
  * Let prosec be a Pro/E section.
  * Suppose optconv finds a subsection of that it
  * can convert and let prosubsec be this proe section.
	* Let swsubsec be the SW section that prosubsec
	* is converted to.
	* optconv.apply(prosec) should return Some(prosubsec, swsubsec).
	* If no subsection could be converted,
	* then None is returned.
  */
trait Converter extends Function1[Pro2D, Option[(Pro2D, SW2D)]]

/** Allows a Java class to easierly implement
  * a subclass of Converter by having clients
  * implement noOptApply instead of apply,
  * which, does not return scala.None.
  * Instead of returning None,
  * noOptApply should return null
  */
abstract class NonOptConverter extends Converter
{
	def apply(prosec:Pro2D):Option[(Pro2D, SW2D)] = {
		val sectionPair = nonOptApply(prosec)
		if(sectionPair != null)
			Some(sectionPair)
		else
			None
	}
	
	def nonOptApply(prosec:Pro2D):(Pro2D, SW2D)
}


class DefaultConversion(prosec:Pro2D) extends OneToOneConv
{
	def convSection:(Pro2D, SW2D) = {
		// First, try to perform a one-to-one conversion for each of the 2D elements
		val proSWEntPairs:List[(proe.Entity, sw.Entity)] =
			Utils.convElems(prosec.entities, optconvert)
		val proSWConPairs:List[(proe.Constraint, sw.Constraint)] =
			Utils.convElems(prosec.constraints, optconvert)
		val proSWDimPairs:List[(proe.Dimension, sw.Dimension)] =
			Utils.convElems(prosec.dimensions, optconvert)
		
		// remaining proe section to convert
		var remainingProSec = Pro2D(
			prosec.entities    -- proSWEntPairs.map(_._1),
			prosec.constraints -- proSWConPairs.map(_._1),
			prosec.dimensions  -- proSWDimPairs.map(_._1)
		)
		println(remainingProSec)
		//println(outputSWSec)
		
		// output SW section so far
		var outputSWSec = SW2D(proSWEntPairs.map(_._2),
			proSWConPairs.map(_._2), proSWDimPairs.map(_._2))
		
		// remove Pro/E axis lines from entities list
		remainingProSec = remainingProSec filterEnts {
			e:proe.Entity => e match {
				case proe.Line(_, true, _, _) => false
				case _ => true
			}
		}

		// convert combination of constraints
		
		// search for ConvPntOnEntIntersect Combination
		val convPntOnEntIntersectSec = new ConvPntOnEntIntersect

		// convert line-point dimensions to pro/e axes
		val convLinePointDimToAxesSec = new ConvLinePointDimToAxes
		
		val converters = List(convPntOnEntIntersectSec, convLinePointDimToAxesSec)

		// val convPntOnEntIntersectSec = new ConvPntOnEntIntersect with LoggingConverter
		// Utils.convertSubsection(remainingProSec, convPntOnEntIntersectSec) match {
		Utils.convertSubsection(remainingProSec, converters) match {
			case Some((prosubsec, swsubsec)) =>
				remainingProSec = remainingProSec -- prosubsec
				outputSWSec = outputSWSec ++ swsubsec
			case None => () // do nothing
		}
		
		// Lastly copy the name of the section
		outputSWSec.name = prosec.name

		// All conversion funcs applied so return
		println(remainingProSec)
		println(outputSWSec)
		(prosec -- remainingProSec, outputSWSec)
	}


	// converting entities

	def convLine(line:proe.Line) =
		if(!line.isProj)
			new sw.Line(entId(line), convPoint(line.start), convPoint(line.end))
		else
			throw new UnMappedElementException(line)
	
	//--------------------------------------------------------------------------------
	def convSpline(spline:proe.Spline) ={
		  
	  val swpoints = spline.points.map(convPoint)
	  new sw.Spline(entId(spline), swpoints)
	 
		} 
	//--------------------------------------------------------------------------------
	
	def convPointEntity(epnt:proe.PointEntity) = convPoint(epnt.point)
	
	def convPoint(pnt:proe.Point) =
		new sw.Point(entId(pnt), pnt.x, pnt.y, defaultZ)
	
	private val defaultZ = 0

	// converting constraints
	
	def convSamePoint(con:proe.SamePoint) =
		new sw.Coincident(convPoint(con.point1), convPoint(con.point2))

	def convHorizontalConstraint(con:proe.HorizontalConstraint) =
		new sw.HorizontalConstraint(convLine(con.line))

	def convVerticalConstraint(con:proe.VerticalConstraint) =
		new sw.VerticalConstraint(convLine(con.line))

	// converting dimensions

	def convLineDim(dim:proe.LineDim) =
		new sw.LineDim(dimId(dim), dim.value,
			convPoint(dim.line.start), convPoint(dim.line.end))

	def convLinePointDim(dim:proe.LinePointDim) =
		new sw.LineDim(dimId(dim), dim.value,
			convLine(dim.line), convPoint(dim.point))
			

	/** Converts subsections based on the INTERSECT rule */
	class ConvPntOnEntIntersect extends Converter
	{
		import Utils.proeLineToLine2D
		import util._
		import util.MathFun._
	
		def apply(prosec:Pro2D):Option[(Pro2D, SW2D)] = {
			var cons1 = prosec.constraints
			while(!cons1.isEmpty) {
				cons1.head match {
					case proe.PntOnEnt(id1, line1, point1) =>
						var cons2 = cons1.tail
						while(!cons2.isEmpty) {
							cons2.head match {
								case proe.PntOnEnt(id2, line2, point2) =>
									if(id1 != id2 && point1 == point2) {
										intersection(line1, line2) match {
											case Some(Point2D(x, y)) =>
												// create a SW point entity at the intersection point
												val swIntersectPoint = createPoint(x, y)
												val swentities = List(swIntersectPoint)
												// convert point referred to by proe.PntOnEnt constraints
												val swReferredPoint = convPoint(point1)
												// create coincident constraint to constraint the intersection
												// and the referred point to be coincident
												val swconstraints = List(sw.Coincident(swIntersectPoint, swReferredPoint))
												return Some(
													(Pro2D(Nil, List(cons1.head, cons2.head), Nil),
													 SW2D(swentities, swconstraints, Nil)))
											case None => () // lines do not intersect at a unique point
										}
									}
								case _ => ()
							}
							cons2 = cons2.tail
						}
					case _ => ()
				}
				cons1 = cons1.tail
			}
			None
		}
	}


	/** Converts line-points dimensions referencing axis lines */
	class ConvLinePointDimToAxes extends Converter
	{
		import Utils.proeLineToLine2D
		import util._
		import util.MathFun._

		def convertLinePointDimToAxes(dim:proe.LinePointDim):Option[sw.Dimension] =
			if(isXAxis(dim.line))
				Some(new sw.VerticalLineDim(dimId(dim), dim.value,
					origin, convPoint(dim.point)))
			else if(isYAxis(dim.line))
				Some(new sw.HorizontalLineDim(dimId(dim), dim.value,
					origin, convPoint(dim.point)))
			else
				None

		def apply(prosec:Pro2D):Option[(Pro2D, SW2D)] = {
			// get the line-point dimensions
			val lpdims:List[proe.LinePointDim] =
				for(d <- prosec.dimensions if d.isInstanceOf[proe.LinePointDim])
					yield d.asInstanceOf[proe.LinePointDim]
			val proswdims:List[(proe.LinePointDim, sw.Dimension)] =
				Utils.convElems(lpdims, convertLinePointDimToAxes)
			if(!proswdims.isEmpty)
				Some((Pro2D(Nil, Nil, proswdims.map(_._1)),
				      SW2D(List(origin), Nil, proswdims.map(_._2))))
			else
				None
		}
	}

	
	def entId(ent:proe.Entity) = (ent.id, 0)
	
	def entId(point:proe.Point):(Int,Int) = point.ent match {
		case proe.Line(id, _, start, end) =>
			if(start == point) (id, 1)
			else (id, 2)
		case pent:proe.PointEntity => entId(pent)
		case proe.Spline(id, _, points, _, _) => {
		  val index = points indexOf point
		  if(index == -1) {
		    throw new IllegalConversionException(
		        "%s not found in %s" format (point, points))
		  }
		  (id, index+1)
		}
	}
	
	def dimId(dim:proe.Dimension) = "D" + dim.id

	def createPoint(x:Double, y:Double) =
		new sw.Point(freshEntId, x, y, defaultZ)

	private def freshEntId:(Int, Int) = {
			val newFirstId = usedFirstIds.max + 1
			usedFirstIds add newFirstId
			(newFirstId, 0)
		}

	private val usedFirstIds = new util.ListMax[Int]
	 
	usedFirstIds ++= prosec.entities.map(_.id)
	
	val origin = createPoint(0, 0)
}

class IllegalConversionException(msg:String) extends RuntimeException(msg)

class UnMappedElementException(elem:proe.Pro2DElem, msg:String) extends
	RuntimeException(msg)
{
	def this(elem:proe.Pro2DElem) =
		this(elem, "The following Pro/E 2D element could not be converted: " + elem)
}

object Utils
{
	/** Tries to convert all A elems in list l to B elems.
	  * If an elem a in A was converted to elem b, then
	  * (a,b) is in the list returned; otherwise,
	  * elem a is skipped.
	  */
	def convElems[A,B](l:List[A], optconv:A=>Option[B]):List[(A,B)] = l match {
		case a::as => optconv(a) match {
			case Some(b) => (a, b) :: convElems(as, optconv)
			case None => convElems(as, optconv)
		}
		case Nil => Nil
	}
	
	/** Searches for a subsection of prosec that the function
	  * optconv can convert.
	  * This function keeps reapplying optconv until it cannot
	  * find a subsection to convert anymore.
	  * Suppose optconv finds a subsection,
	  * and let prosubsec be this proe section.
	  * Let swsubsec be the SW section that prosubsec
	  * is converted to.
	  * This function will return Some(prosec -- prosubsec, swsubsec).
	  * If no subsection could be converted,
	  * then None is returned.
	  */
	def convertSubsection(prosec:Pro2D, optconv:Converter):Option[(Pro2D,SW2D)] =
	{
		var remainingProSec = prosec
		var outputSWSec = SW2D.emptySection
		var subsectionFound = false
		var stillConverting = true
		while(stillConverting) {
			optconv(remainingProSec) match {
				case Some((prosubsec, swsubsec)) =>
					if(prosubsec.isEmpty) {
						throw new IllegalConversionException(
							"optconv returned an empty Pro/E subsection")
					}
					subsectionFound = true
					remainingProSec = remainingProSec -- prosubsec
					outputSWSec = outputSWSec ++ swsubsec
				case None =>
					stillConverting = false
			}
		}
		if(subsectionFound)
			return Some((prosec -- remainingProSec, outputSWSec))
		else
			return None
	}
	
	def convertSubsection(prosec:Pro2D, converters:List[Converter]):Option[(Pro2D, SW2D)] = {
		var stillConverting = true
		var remainingProSec = prosec
		var outputSWSec = SW2D.emptySection
		var subsectionFound = false
		for(optconv <- converters) {
			convertSubsection(remainingProSec, optconv) match {
				case Some((prosubsec, swsubsec)) =>
					remainingProSec = remainingProSec -- prosubsec
					outputSWSec = outputSWSec ++ swsubsec
					subsectionFound = true
				case None => () // do nothing
			}
		}
		if(subsectionFound)
			Some(prosec -- remainingProSec, outputSWSec)
		else
			None
	}
	
	import util._
	
	implicit def proePntToPoint2D(ppnt:proe.Point):Point2D =
		Point2D(ppnt.x, ppnt.y)
	
	implicit def proeLineToLine2D(pline:proe.Line):Line2D =
		Line2D(pline.start, pline.end)
}


trait LoggingConverter extends Converter
{
	abstract override def apply(prosec:Pro2D):Option[(Pro2D, SW2D)] =
		super.apply(prosec) match {
			case somepair @ Some((prosubsec, swsubsec)) =>
				println("Pro/E 2D subsection conversion performed")
				println("Pro/E subsection:")
				println(prosubsec)
				println("Output SolidWorks subsection:")
				println(swsubsec)
				somepair
			case None => None
		}
}


object Tester
{
	import pxmlparser._

	def main(args:Array[String]):Unit = {
		if(args.length == 0) {
			Console.err.println("usage: scala psconv.pro2dtosw.Tester <Pro/E section XML file>")
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
		println
		println("Output SolidWorks section:")
		println(swsection)
	}
}


