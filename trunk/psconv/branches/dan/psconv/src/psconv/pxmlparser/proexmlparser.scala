package psconv.pxmlparser
import psconv.proe._
import scala.xml._
import java.io.File
import String.format

trait Node2ProeFactory
{
	def createEntity(node:Node):Entity
	def createConstraint(node:Node):Constraint
	def createDimension(node:Node):Dimension
}

class ProeXMLParser(file:File) extends Node2ProeFactory
{
	import Utils._

	def this(filepath:String) = this(new File(filepath))

	val rootElem:Elem = XML.loadFile(file)
	
	def createSection:Pro2D = {
		val sec = Pro2D(getEntities, getConstraints, getDimensions)
		sec.name = getAttrVal(rootElem, "name")
		sec
	}
	
	def getEntityNodes =
		rootElem \ "pro2dEntities" \ "pro2dEntity"

	def getConstraintNodes =
		rootElem \ "pro2dConstraints" \ "pro2dConstraint"

	def getDimensionNodes =
		rootElem \ "pro2dDimensions" \ "pro2dDimension"

	def getEntities:List[Entity] =
		// getEntityNodes map createEntity toList
		mapThrowingFunc(getEntityNodes, createEntity)
	
	def getConstraints:List[Constraint] =
		getConstraintNodes map createConstraint toList
		// mapThrowingFunc(getConstraintNodes, createConstraint)
	
	def getDimensions:List[Dimension] =
		getDimensionNodes map createDimension toList
		// mapThrowingFunc(getDimensionNodes, createDimension)

	def createEntity(node:Node):Entity = getAttrVal(node, "type") match {
		case "PRO_2D_POINT" => createPointEnt(node)
		case "PRO_2D_LINE"  => createLine(node)
		case "PRO_2D_SPLINE" => createSpline(node)
		case "PRO_2D_CIRCLE" => createCircle(node)
		case "PRO_2D_ARC" => createArc(node)
		case enttype =>
			throw new ParserException("Unknown entity type: " + enttype)
	}

	def createConstraint(node:Node):Constraint = getAttrVal(node, "type") match {
		case "PRO_CONSTRAINT_SAME_POINT" => createSamePoint(node)
		case "PRO_CONSTRAINT_HORIZONTAL_ENT" => createHorizontalConstraint(node)
		case "PRO_CONSTRAINT_VERTICAL_ENT" => createVerticalConstraint(node)
		case "PRO_CONSTRAINT_PNT_ON_ENT" => createPntOnEnt(node)
		case "PRO_CONSTRAINT_EQUAL_RADII" => createEqualRadii(node)
		case "PRO_CONSTRAINT_EQUAL_SEGMENTS" => createEqualSegments(node)
		case contype =>
			throw new ParserException("Unknown constraint type: " + contype)
	}

	def createDimension(node:Node):Dimension = getAttrVal(node, "type") match {
		case "PRO_TK_DIM_LINE" => createLineDim(node)
		case "PRO_TK_DIM_LINE_POINT" => createLinePointDim(node)
				case "PRO_TK_DIM_DIA" => createDiamDim(node)
		case "PRO_TK_DIM_RAD" => createRadiusDim(node)
		case "PRO_TK_DIM_LINES_ANGLE" => createAngleDim(node)
		case dimtype =>
			throw new ParserException("Unknown dimension type: " + dimtype)
	}

	/** Expecting XML node with format:
	<pre>
		< pro2dEntity id="7" isProjection="true" type="PRO_2D_POINT">
  			<pnt>
    			< Pro2dPnt x="3.5" y="4.7"/>
  			</pnt>
		</pro2dEntity>
	</pre>
	 */
	def createPointEnt(node:Node):PointEntity = {
		val point = createPoint(getUniqueNode(node \ "pnt" \ "Pro2dPnt"))
		PointEntity(getId(node), point)
	}

	/** Expecting XML node with format: < Pro2dPnt x="3.5" y="4.7" /> */
	def createPoint(node:Node):Point = {
		val x = getAttrVal(node, "x").toDouble
		val y = getAttrVal(node, "y").toDouble
		Point(x, y)
	}
	
	/** Expecting XML node with format <radius>0.50</radius> */
	def createRadius(node:Node):Double = node.text.toDouble

	/** Expecting XML node with format:
	<start_angle>
        <ProAngle radians="0.40" />
      </start_angle>
      <end_angle>
        <ProAngle radians="1.42" />
      </end_angle>
	  */
	def createAngle(node:Node):Angle = {
		val angle = getAttrVal(node, "radians").toDouble
		Angle(angle)
	}	
	
	/** Expecting XML node with format:
	<pre>
  		<pro2dEntity id ="7" isProjection="true" type="PRO_2D_LINE">
    		<end1>
      			<Pro2dPnt x="3.5" y="4.7"/>
    		</end1>
    		<end2>
      			<Pro2dPnt x="3.5" y="4.7"/>
    		</end2>
  		</pro2dEntity>
	</pre>
	*/
		
	def createLine(node:Node):Line = {
		val start = createPoint(getUniqueNode(node \ "end1" \ "Pro2dPnt"))
		val end = createPoint(getUniqueNode(node \ "end2" \ "Pro2dPnt"))
		val isProj = getAttrVal(node, "isProjection").toBoolean
		Line(getId(node), isProj, start, end)
	}
	
	/** Expecting XML node with format:
	<pre>
		<pro2dEntity id="4" isProjection="false" type="PRO_2D_CIRCLE" >
			<center>
				<Pro2dPnt x="0.50" y="0.50" />
			</center>
			<radius>0.50</radius>
		</pro2dEntity>
	</pre>
	*/
	def createCircle(node:Node):Circle = {
		val middle = createPoint(getUniqueNode(node \ "center" \ "Pro2dPnt"))
		val rad = createRadius(getUniqueNode(node))
		Circle(getId(node), rad, middle)
	}
	
	/** Expecting XML node with format:
    <pro2dEntity id="16" isProjection="false" type="PRO_2D_ARC" >
      <center>
        <Pro2dPnt x="10.00" y="15.00" />
      </center>
      <start_angle>
        <ProAngle radians="0.40" />
      </start_angle>
      <end_angle>
        <ProAngle radians="1.42" />
      </end_angle>
      <radius>12.92</radius>
    </pro2dEntity>
	*/
	def createArc(node:Node):Arc = {
		val middle = createPoint(getUniqueNode(node \ "center" \ "Pro2dPnt"))
		val start_angle = createAngle(getUniqueNode(node \"start_angle" \ "ProAngle"))
		val end_angle = createAngle(getUniqueNode(node \"end_angle" \ "ProAngle"))
		val rad = createRadius(getUniqueNode(node))
		val startPt = createArcEndpoint(middle, start_angle, rad)
		val endPt = createArcEndpoint(middle, end_angle, rad)
		Arc(getId(node), middle, start_angle, end_angle, rad, startPt, endPt)
	}
	
	def createArcEndpoint(middle:Point, angle:Angle, radius:Double):Point = {
		val ptX = middle.x + radius * Math.cos(angle.radians)
		val ptY = middle.y + radius * Math.sin(angle.radians)
		Point(ptX, ptY)
	}

	def createEntityObj(entId:Int, pointType:String):EntityObj = {
		val entNode = getUniqueNode(
			getEntityNodes.filter(n => (n \ "@id").text.toInt == entId))
		(createEntity(entNode), pointType) match {
			case (line:Line, "PRO_ENT_WHOLE") => line
			case (Line(_, _, start, _), "PRO_ENT_START") => start
			case (Line(_, _, _, end), "PRO_ENT_END") => end
			case (PointEntity(_, pnt), "PRO_ENT_WHOLE") => pnt
			case (circle:Circle, "PRO_ENT_WHOLE") => circle
			case (Circle(_, _, center), "PRO_ENT_CENTER") => center
			case (arc:Arc, "PRO_ENT_WHOLE") => arc
			case (Arc(_, center, _, _, _, _, _), "PRO_ENT_CENTER") => center
			case (Arc(_, _, start_angle, _, _, _, _), "PRO_ENT_START_ANGLE") => start_angle
			case (Arc(_, _, _, end_angle, _, _, _), "PRO_ENT_END_ANGLE") => end_angle
			// for arcs, create start and end points?
			case (Arc(_, _, _, _, _, start, _), "PRO_ENT_START") => start
			case (Arc(_, _, _, _, _, _, end), "PRO_ENT_END") => end
			case (Spline(_, _, points, _, _), "PRO_ENT_START") => points(0)
			case (Spline(_, nPoints, points, _, _), "PRO_ENT_END") => points(nPoints - 1)
			case (spline:Spline, "PRO_ENT_WHOLE") => spline
			case (ent, _) =>
				throw new ParserException("Unknown (entity, point type) " +
					"combination: " + (ent, pointType))
		}
	}

	/** Expecting XML node with format:
	<pre>
		<pro2dConstraint id="0" type="PRO_CONSTRAINT_SAME_POINT">
		  <entityReferences>
		    <entityReference id ="4"/>
		    <entityReference id ="5"/>
		  </entityReferences>
		  <pointTypes>
		    <pointType type ="PRO_ENT_END"/>
		    <pointType type ="PRO_ENT_START"/>
		  </pointTypes>
		</pro2dConstraint>
	</pre>
	*/
	def createSamePoint(node:Node):SamePoint = {
		val refs = node \ "entityReferences" \ "entityReference"
		assert(refs.length == 2)
		val ptypes = node \ "pointTypes" \ "pointType"
		assert(ptypes.length == 2)
		val point0 = createEntityObj(getId(refs(0)),
			getAttrVal(ptypes(0), "type")).asInstanceOf[Point]
		val point1 = createEntityObj(getId(refs(1)),
			getAttrVal(ptypes(1), "type")).asInstanceOf[Point]
		SamePoint(getId(node), point0, point1)
	}

	/** Expecting XML node with format:
	<pre>
		<pro2dConstraint id="4" type="PRO_CONSTRAINT_HORIZONTAL_ENT">
		  <entityReferences>
		    <entityReference id ="4"/>
		  </entityReferences>
		  <pointTypes>
		    <pointType type ="PRO_ENT_WHOLE"/>
		  </pointTypes>
		</pro2dConstraint>
	</pre>
	*/
	def createHorizontalConstraint(node:Node):HorizontalConstraint = {
		val refs = node \ "entityReferences" \ "entityReference"
		assert(refs.length == 1)
		val ptypes = node \ "pointTypes" \ "pointType"
		assert(ptypes.length == 1)
		val line = createEntityObj(getId(refs(0)),
			getAttrVal(ptypes(0), "type")).asInstanceOf[Line]
		HorizontalConstraint(getId(node), line)
	}

	/** Expecting XML node with format:
	<pre>
		<pro2dConstraint id="4" type="PRO_CONSTRAINT_VERTICAL_ENT">
		  <entityReferences>
		    <entityReference id ="4"/>
		  </entityReferences>
		  <pointTypes>
		    <pointType type ="PRO_ENT_WHOLE"/>
		  </pointTypes>
		</pro2dConstraint>
	</pre>
	*/
	def createVerticalConstraint(node:Node):VerticalConstraint = {
		val refs = node \ "entityReferences" \ "entityReference"
		assert(refs.length == 1)
		val ptypes = node \ "pointTypes" \ "pointType"
		assert(ptypes.length == 1)
		val line = createEntityObj(getId(refs(0)),
			getAttrVal(ptypes(0), "type")).asInstanceOf[Line]
		VerticalConstraint(getId(node), line)
	}
	
	/** Expecting XML node with format:
	<pre>
		<pro2dConstraint id="8" type="PRO_CONSTRAINT_PNT_ON_ENT">
		  <entityReferences>
		    <entityReference id ="0"/>
		    <entityReference id ="4"/>
		  </entityReferences>
		  <pointTypes>
		    <pointType type ="PRO_ENT_WHOLE"/>
		    <pointType type ="PRO_ENT_START"/>
		  </pointTypes>
		</pro2dConstraint>
	</pre>
	*/
	def createPntOnEnt(node:Node):PntOnEnt = {
		val refs = node \ "entityReferences" \ "entityReference"
		assert(refs.length == 2)
		val ptypes = node \ "pointTypes" \ "pointType"
		assert(ptypes.length == 2)
		val line = createEntityObj(getId(refs(0)),
			getAttrVal(ptypes(0), "type")).asInstanceOf[Line]
		val point = createEntityObj(getId(refs(1)),
			getAttrVal(ptypes(1), "type")).asInstanceOf[Point]
		PntOnEnt(getId(node), line, point)
	}
	
	/** Expecting XML node with format:
	<pre>
		<pro2dConstraint id="0" type="PRO_CONSTRAINT_EQUAL_RADII" status="PRO_TK_CONSTRAINT_ENABLED">
	      <entityReferences>
	        <entityReference id ="4" />
	        <entityReference id ="7" />
	      </entityReferences>
	      <pointTypes>
	        <pointType type ="PRO_ENT_WHOLE" />
	        <pointType type ="PRO_ENT_WHOLE" />
	      </pointTypes>
	    </pro2dConstraint>
	</pre>
	 */
	def createEqualRadii(node:Node):EqualRadii = {
	  val refs = node \ "entityReferences" \ "entityReference"
	  assert(refs.length == 2)
	  val ptypes = node \ "pointTypes" \ "pointType"
	  assert(ptypes.length == 2)
	  val circle1 = createEntityObj(getId(refs(0)),
	      getAttrVal(ptypes(0), "type")).asInstanceOf[Circle]
	  val circle2 = createEntityObj(getId(refs(1)),
	      getAttrVal(ptypes(1), "type")).asInstanceOf[Circle]
	  EqualRadii(getId(node), circle1, circle2)
	}
	
	/**
    <pro2dConstraint id="9" type="PRO_CONSTRAINT_EQUAL_SEGMENTS" status="PRO_TK_CONSTRAINT_ENABLED">
      <entityReferences>
        <entityReference id ="10" />
        <entityReference id ="4" />
      </entityReferences>
      <pointTypes>
        <pointType type ="PRO_ENT_WHOLE" />
        <pointType type ="PRO_ENT_WHOLE" />
      </pointTypes>
    </pro2dConstraint>
	**/
	def createEqualSegments(node:Node):EqualSegments = {
	  val refs = node \ "entityReferences" \ "entityReference"
	  assert(refs.length == 2)
	  val ptypes = node \ "pointTypes" \ "pointType"
	  assert(ptypes.length == 2)
	  val line1 = createEntityObj(getId(refs(0)),
	      getAttrVal(ptypes(0), "type")).asInstanceOf[Line]
	  val line2 = createEntityObj(getId(refs(1)),
	      getAttrVal(ptypes(1), "type")).asInstanceOf[Line]
	  EqualSegments(getId(node), line1, line2)
	}

	/** Expecting XML node with format:
	<pre>
	<pro2dDimension id="1" type="PRO_TK_DIM_LINE" value="200.00" >
	  <entityReferences>
	    <entityReference id ="4"/>
	  </entityReferences>
	  <pointTypes>
	    <pointType type ="PRO_ENT_WHOLE"/>
	  </pointTypes>
	</pro2dDimension>
	</pre>
	*/
	def createLineDim(node:Node):LineDim = {
		val refs = node \ "entityReferences" \ "entityReference"
		assert(refs.length == 1)
		val ptypes = node \ "pointTypes" \ "pointType"
		assert(ptypes.length == 1)
		val line = createEntityObj(getId(refs(0)),
			getAttrVal(ptypes(0), "type")).asInstanceOf[Line]
		val dimValue = getAttrVal(node, "value").toDouble
		val dimLoc = createPoint(getUniqueNode(node \ "location" \ "Pro2dPnt"))
		LineDim(getId(node), dimValue, dimLoc, line)
	}	
	
	/** Expecting XML node with format:
	<pre>
	<pro2dDimension id="3" type="PRO_TK_DIM_LINE_POINT" value="40.00" >
		<entityReferences>
			<entityReference id ="0" />
			<entityReference id ="7" />
		</entityReferences>
		<pointTypes>
			<pointType type ="PRO_ENT_WHOLE"/>
			<pointType type ="PRO_ENT_START"/>
		</pointTypes>
	</pro2dDimension>
	</pre>
	*/
	def createLinePointDim(node:Node):LinePointDim = {
		val refs = node \ "entityReferences" \ "entityReference"
		assert(refs.length == 2)
		val ptypes = node \ "pointTypes" \ "pointType"
		assert(ptypes.length == 2)
		val line = createEntityObj(getId(refs(0)),
			getAttrVal(ptypes(0), "type")).asInstanceOf[Line]
		val point = createEntityObj(getId(refs(1)),
			getAttrVal(ptypes(1), "type")).asInstanceOf[Point]
		val dimValue = getAttrVal(node, "value").toDouble
		val dimLoc = createPoint(getUniqueNode(node \ "location" \ "Pro2dPnt"))
		LinePointDim(getId(node), dimValue, dimLoc, line, point)
	}	
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------
	def createSpline(node:Node):Spline = {    	    
	    val points =
	      (for(n <- node \ "point_arr" \ "Pro2dPnt") yield createPoint(n)).toList

	    val startTangAngle = createAngle(getUniqueNode(node \ "start_tang_angle" \ "ProAngle"))
		val endTangAngle = createAngle(getUniqueNode(node \ "end_tang_angle" \ "ProAngle"))

	    Spline(getId(node), points.length, points, startTangAngle, endTangAngle)
	}//------------------------------------------------------------------------------------------------------------------------------------------------------
	
	/** Expecting XML node with format:
	<pre>
	<pro2dDimension id="0" type="PRO_TK_DIM_DIA" value="1.00" >
      <entityReferences>
        <entityReference id ="4" />
      </entityReferences>
      <pointTypes>
        <pointType type ="PRO_ENT_WHOLE" />
      </pointTypes>
    </pro2dDimension>
	</pre>
	*/
	def createDiamDim(node:Node):DiamDim = {
		val refs = node \ "entityReferences" \ "entityReference"
		assert(refs.length == 1)
		val ptypes = node \ "pointTypes" \ "pointType"
		assert(ptypes.length == 1)
		val circle = createEntityObj(getId(refs(0)),
			getAttrVal(ptypes(0), "type")).asInstanceOf[Circle]
		val dimValue = getAttrVal(node, "value").toDouble
		val dimLoc = createPoint(getUniqueNode(node \ "location" \ "Pro2dPnt"))
		DiamDim(getId(node), dimValue, dimLoc, circle)
	}
	
	/** Expecting XML node with format:
	<pro2dDimension id="8" type="PRO_TK_DIM_RAD" value="12.92" >
	      <entityReferences>
	        <entityReference id ="16" />
	      </entityReferences>
	      <pointTypes>
	        <pointType type ="PRO_ENT_WHOLE" />
	      </pointTypes>
	    </pro2dDimension>
	**/
	def createRadiusDim(node:Node):RadiusDim = {
		val refs = node \ "entityReferences" \ "entityReference"
		assert(refs.length == 1)
		val ptypes = node \ "pointTypes" \ "pointType"
		assert(ptypes.length == 1)
		val arc = createEntityObj(getId(refs(0)),
			getAttrVal(ptypes(0), "type")).asInstanceOf[Arc]
		val dimValue = getAttrVal(node, "value").toDouble
		val dimLoc = createPoint(getUniqueNode(node \ "location" \ "Pro2dPnt"))
		RadiusDim(getId(node), dimValue, dimLoc, arc)
	}
	
	/** Expecting XML with format:
	<pro2dDimension id="3" type="PRO_TK_DIM_LINES_ANGLE" value="45.00" >
      	<entityReferences>
        	<entityReference id ="4" />
        	<entityReference id ="5" />
      	</entityReferences>
      <pointTypes>
        	<pointType type ="PRO_ENT_WHOLE" />
        	<pointType type ="PRO_ENT_WHOLE" />
      </pointTypes>
    </pro2dDimension>
	 **/
	def createAngleDim(node:Node):AngleDim = {
		val refs = node \ "entityReferences" \ "entityReference"
		assert(refs.length == 2)
		val ptypes = node \ "pointTypes" \ "pointType"
		assert(ptypes.length == 2)
		val line1 = createEntityObj(getId(refs(0)),
			getAttrVal(ptypes(0), "type")).asInstanceOf[Line]
		val line2 = createEntityObj(getId(refs(1)),
			getAttrVal(ptypes(1), "type")).asInstanceOf[Line]
		val dimValue = getAttrVal(node, "value").toDouble
		val dimLoc = createPoint(getUniqueNode(node \ "location" \ "Pro2dPnt"))
		AngleDim(getId(node), dimValue, dimLoc, line1, line2)
	}	
}

object Utils
{
	def getAttrVal(node:Node, name:String):String =
		node.attribute(name) match {
			case Some(attr) => attr.text
			case None => throw new MissingAttributeException(node, name)
		}

	def getId(node:Node) = getAttrVal(node, "id").toInt
	
	def mapThrowingFunc[A,B,C<:A](elems:Seq[C], func:A=>B):List[B] =
		elems.toList match {
			case e::es =>
				try { func(e) :: mapThrowingFunc(es, func) }
				catch {
					case exc:ParserException =>
						println("R exc:ParserException =>")
						Console.err.println("Error: " + exc)
						mapThrowingFunc(es, func)
				}
			case Nil => Nil
		}

	def getUniqueNode(seq:Seq[Node]):Node = {
		val itr = seq.iterator
		if(itr.hasNext) {
			val node = itr.next
			if(itr.hasNext)
				throw new MultipleNode(seq)
			else
				node
		}
		else	throw new NoSuchNode(seq)			
	}
}	

class ParserException(msg:String) extends RuntimeException(msg)

class MissingAttributeException(node:Node, name:String) extends ParserException(
	format("Missing attribute \"%s\" from node:%n%s", name, node))

class NoSuchNode(seq:NodeSeq) extends RuntimeException(
	"No nodes found where at least one node was expected")

class MultipleNode(seq:NodeSeq) extends RuntimeException(
	"Multiple nodes found where exactly one node was expected")


trait CachedNode2ProeFactory extends Node2ProeFactory
{
	private val cache = new scala.collection.mutable.HashMap[Node,AnyRef]
	
	abstract override def createEntity(node:Node):Entity =
		cache.get(node) match {
			case Some(obj) => obj.asInstanceOf[Entity]
			case None =>
				val ent = super.createEntity(node)
				cache += node -> ent
				ent
		}

	abstract override def createConstraint(node:Node):Constraint =
		cache.get(node) match {
			case Some(obj) => obj.asInstanceOf[Constraint]
			case None =>
				val con = super.createConstraint(node)
				cache += node -> con
				con
		}

	abstract override def createDimension(node:Node):Dimension =
		cache.get(node) match {
			case Some(obj) => obj.asInstanceOf[Dimension]
			case None =>
				val dim = super.createDimension(node)
				cache += node -> dim
				dim
		}
}

object Tester
{
	def main(args:Array[String]):Unit = {
		if(args.length == 0) {
			Console.err.println("usage: scala psconv.pxmlparser.Tester <Pro/E section XML file>")
			exit(1)
		}
		val parser = new ProeXMLParser(args(0)) with CachedNode2ProeFactory
		val section = parser.createSection
		println("Section found:")
		println(section)
	}
}

