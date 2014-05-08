package psconv.proe

case class Pro2D(val entities:List[Entity],
	val constraints:List[Constraint], val dimensions:List[Dimension])
{
	var name = "ProeSection"

	def ++(other:Pro2D) =
		Pro2D(entities    ++ other.entities,
		      constraints ++ other.constraints,
		      dimensions  ++ other.dimensions)

	def addEnts(otherEntities:List[Entity]) =
		Pro2D(entities ++ otherEntities, constraints, dimensions)

	def addCons(otherConstraints:List[Constraint]) =
		Pro2D(entities, constraints ++ otherConstraints, dimensions)

	def addDims(otherDimensions:List[Dimension]) =
		Pro2D(entities, constraints, dimensions ++ otherDimensions)


	def --(other:Pro2D) =
		Pro2D(entities filterNot (other.entities contains),
		      constraints filterNot (other.constraints contains),
		      dimensions filterNot (other.dimensions contains))
		      
	def subEnts(otherEntities:List[Entity]) =		
	    Pro2D(entities filterNot (otherEntities contains), constraints, dimensions)

	def subCons(otherConstraints:List[Constraint]) =	  	
		Pro2D(entities, constraints filterNot (otherConstraints contains), dimensions)
		
	def subDims(otherDimensions:List[Dimension]) =	  	
		Pro2D(entities, constraints, dimensions filterNot (otherDimensions contains))

	def filterEnts(p: Entity => Boolean) =
		Pro2D(entities filter p, constraints, dimensions)

	def filterCons(p: Constraint => Boolean) =
		Pro2D(entities, constraints filter p, dimensions)

	def filterDims(p: Dimension => Boolean) =
		Pro2D(entities, constraints, dimensions filter p)
	
	def sameLengths(other:Pro2D):Boolean =
		(entities.length    == other.entities.length) &&
		(constraints.length == other.constraints.length) &&
		(dimensions.length   == other.dimensions.length)
	
	def isEmpty = entities.isEmpty && constraints.isEmpty && dimensions.isEmpty
}

object Pro2D
{
	def emptySection = Pro2D(Nil, Nil, Nil)
}

/** root trait of all Pro2D Elements */
trait Pro2DElem

/** The EntityObj type represents the set of elements that
  * can be referred to by entity references.
  */
sealed trait EntityObj extends Pro2DElem

// ProE 2D Entities
sealed abstract class Entity(val id:Int) extends EntityObj

// <pro2dEntity id="0" isProjection="true" type="PRO_2D_LINE" >
case class Line(override val id:Int, val isProj:Boolean, val start:Point,
	val end:Point) extends Entity(id)
{
	start.ent = this
	end.ent = this
}

case class Circle(override val id:Int, val radius:Double, val center:Point) extends Entity(id)
{
	center.ent = this
}

case class Arc(override val id:Int, val center:Point, val start_angle:Angle,
				val end_angle:Angle, val radius:Double, val start:Point, val end:Point) extends Entity(id)
{
	center.ent = this
	start.ent = this
	end.ent = this
}

case class PointEntity(override val id:Int, val point:Point) extends Entity(id)
{
	point.ent = this
}

// <Pro2dPnt x="0.00" y="0.00" />
/** ent is the entity that this point is a parameter of */
case class Point(x:Double, y:Double) extends EntityObj
{
	var ent:Entity = null
}

// <ProAngle radians="0.40" />
case class Angle(radians:Double) extends EntityObj
{
	var ang:Angle = null
}

case class Spline(override val id:Int, val nPoints:Int, val points:List[Point], val startTangAngle:Angle, val endTang_angle:Angle)extends Entity(id){ 
	var i = 0
	while (i < nPoints){
		points(i).ent = this
		i += 1
	}
  
}
// Entity Reference
// <entityReferences>
// 	<entityReference id ="4" />
// </entityReferences>
// <pointTypes>
// 	<pointType type ="PRO_ENT_END" />
// </pointTypes>
// case class EntityRef(id:Int, ptype:PointType)


// PointTypes - Defined in ProSecdimType.
sealed trait PointType
case object PRO_ENT_WHOLE          extends PointType
case object PRO_ENT_START          extends PointType
case object PRO_ENT_END            extends PointType
case object PRO_ENT_CENTER         extends PointType
case object PRO_ENT_LEFT_TANGENT   extends PointType
case object PRO_ENT_RIGHT_TANGENT  extends PointType
case object PRO_ENT_TOP_TANGENT    extends PointType
case object PRO_ENT_BOTTOM_TANGENT extends PointType


// ProE 2D Constraints
sealed abstract class Constraint(val id:Int) extends Pro2DElem

// <pro2dConstraint id="3" type="PRO_CONSTRAINT_SAME_POINT">
case class SamePoint(override val id:Int, point1:Point, point2:Point) extends
	Constraint(id)

// <pro2dConstraint id="4" type="PRO_CONSTRAINT_HORIZONTAL_ENT">
case class HorizontalConstraint(override val id:Int, line:Line) extends Constraint(id)

// <pro2dConstraint id="6" type="PRO_CONSTRAINT_VERTICAL_ENT">
case class VerticalConstraint(override val id:Int, line:Line) extends Constraint(id)

// <pro2dConstraint id="8" type="PRO_CONSTRAINT_PNT_ON_ENT"
case class PntOnEnt(override val id:Int, line:Line, point:Point) extends Constraint(id)

// <pro2dConstraint id="0" type="PRO_CONSTRAINT_EQUAL_RADII"
case class EqualRadii(override val id:Int, circle1:Circle, circle2:Circle) extends Constraint(id)

// <pro2dConstraint id="9" type="PRO_CONSTRAINT_EQUAL_SEGMENTS"
case class EqualSegments(override val id:Int, line1:Line, line2:Line) extends Constraint(id)

// ProE 2D Dimensions
sealed abstract class Dimension(val id:Int, val value:Double, val loc:Point) extends Pro2DElem

// <pro2dDimension id="1" type="PRO_TK_DIM_LINE" value="200.00" >
case class LineDim(override val id:Int, override val value:Double,
	override val loc:Point, line:Line) extends Dimension(id, value, loc)

// <pro2dDimension id="6" type="PRO_TK_DIM_LINE_POINT" value="20.00" >
case class LinePointDim(override val id:Int, override val value:Double,
	override val loc:Point, line:Line, point:Point) extends Dimension(id, value, loc)

// <pro2dDimension id="0" type="PRO_TK_DIM_DIA" value="1.00" >	
case class DiamDim(override val id:Int, override val value:Double,
	override val loc:Point, circle:Circle) extends Dimension(id, value, loc)

// <pro2dDimension id="8" type="PRO_TK_DIM_RAD" value="12.92" >
case class RadiusDim(override val id:Int, override val value:Double,
	override val loc:Point, arc:Arc) extends Dimension(id, value, loc)

// <pro2dDimension id="3" type="PRO_TK_DIM_LINES_ANGLE" value="45.00" >
case class AngleDim(override val id: Int, override val value:Double,
    override val loc:Point, line1:Line, line2:Line) extends Dimension(id, value, loc)
