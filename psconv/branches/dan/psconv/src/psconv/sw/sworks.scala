package psconv.sw

case class SW2D(entities:List[Entity],
	constraints:List[Constraint], dimensions:List[Dimension])
{
	var name = "SolidWorksSection"

	def ++(other:SW2D) =
		SW2D(entities      ++ other.entities,
		      constraints  ++ other.constraints,
		      dimensions   ++ other.dimensions)

	def addEnts(otherEntities:List[Entity]) =
		SW2D(entities ++ otherEntities, constraints, dimensions)

	def addCons(otherConstraints:List[Constraint]) =
		SW2D(entities, constraints ++ otherConstraints, dimensions)

	def addDims(otherDimensions:List[Dimension]) =
		SW2D(entities, constraints, dimensions ++ otherDimensions)


	def --(other:SW2D) =
		SW2D(entities filterNot (other.entities contains),
			 constraints filterNot (other.constraints contains),
			 dimensions filterNot (other.dimensions contains))
			 
	def subEnts(otherEntities:List[Entity]) =
		SW2D(entities filterNot (otherEntities contains), constraints, dimensions)
	  
	def subCons(otherConstraints:List[Constraint]) =
	   	SW2D(entities, constraints filterNot (otherConstraints contains), dimensions)

	def subDims(otherDimensions:List[Dimension]) =
	  	SW2D(entities, constraints, dimensions filterNot (otherDimensions contains))

	def filterEnts(p: Entity => Boolean) =
		SW2D(entities filter p, constraints, dimensions)

	def filterCons(p: Constraint => Boolean) =
		SW2D(entities, constraints filter p, dimensions)

	def filterDims(p: Dimension => Boolean) =
		SW2D(entities, constraints, dimensions filter p)
	
	def sameLengths(other:SW2D):Boolean =
		(entities.length    == other.entities.length) &&
		(constraints.length == other.constraints.length) &&
		(dimensions.length  == other.dimensions.length)
	
	def isEmpty = entities.isEmpty && constraints.isEmpty && dimensions.isEmpty
}

object SW2D
{
	def emptySection = SW2D(Nil, Nil, Nil)
}

sealed abstract class Entity(val id:(Int,Int))

// <sw2DEntity ID="(1,1)" type="swSketchLINE">
case class Line(override val id:(Int,Int), start:Point, end:Point) extends
	Entity(id)

//<sw2DEntity ID="(0,1)" type="swSketchARC">
case class Circle(override val id:(Int,Int), middle:Point, radius:Double) extends
	Entity(id)

case class Arc(override val id:(Int,Int), center:Point, start:Point, end:Point,
    direction:Short) extends Entity(id)

//case class Angle(override val id:(Int,Int), radians:Double) extends Entity(id)

//new spline class-------------------------------------------------------------------
case class Spline(override val id:(Int,Int), points:List[Point]) extends Entity(id)
//-----------------------------------------------------------------------------------

// <sw2DPt ID="(0,1)" x ="0" y ="0" z ="0" />
case class Point(override val id:(Int,Int), x:Double, y:Double,
	z:Double) extends Entity(id)

// Point where a dimension should be written
case class DimPoint(x:Double, y:Double, z:Double)

// SolidWorks 2D Constraints
sealed trait Constraint

// <sw2DConstraint type="swConstraintType_COINCIDENT">
case class Coincident(point1:Point, point2:Point) extends
	Constraint

// <sw2DConstraint type="swConstraintType_HORIZONTAL">
case class HorizontalConstraint(line:Line) extends Constraint

// <sw2DConstraint type="swConstraintType_VERTICAL">
case class VerticalConstraint(line:Line) extends Constraint

// <sw2DConstraint type="swConstraintType_DISTANCE">
case class DistanceConstraint(point1:Point, point2:Point) extends
	Constraint

// <sw2DConstraint type="swConstraintType_DIAMETER">
case class DiameterConstraint(circle:Circle) extends Constraint

// <sw2DConstraint type="swConstraintType_SAMERADII">
case class EqualSize(entity1:Entity, entity2:Entity) extends
	Constraint	
	
// ProE 2D Dimensions
sealed abstract class Dimension(val id:String, val value:Double, val loc:DimPoint)

// <sw2DDimension ID="D0" type="swLinearDimension" Value="100.0">
case class LineDim(override val id:String, override val value:Double,
	override val loc:DimPoint, ent1:Entity, ent2:Entity)
	extends	Dimension(id, value, loc)

// <sw2DDimension ID="D0" type="swDiameterDimension" Value="100.0">
case class DiamDim(override val id:String, override val value:Double,
	override val loc:DimPoint, ent:Entity) extends	Dimension(id, value, loc)

// <sw2DDimension ID="D0" type="swHorLinearDimension" Value="100.0">
case class HorizontalLineDim(override val id:String, override val value:Double,
	override val loc:DimPoint, ent1:Entity, ent2:Entity) 
	extends	Dimension(id, value, loc)

// <sw2DDimension ID="D0" type="swRadialDimension" Value="100.0">
case class RadialDim(override val id:String, override val value:Double,
	override val loc:DimPoint, ent:Entity) extends	Dimension(id, value, loc)

// <sw2DDimension ID="D0" type="swVertLinearDimension" Value="100.0">
case class VerticalLineDim(override val id:String, override val value:Double,
	override val loc:DimPoint, ent1:Entity, ent2:Entity) 
	extends	Dimension(id, value, loc)

// <sw2DDimension ID="D0" type="swAngularDimension" Value="100.0">
case class AngleDim(override val id:String, override val value:Double,
    override val loc:DimPoint, ent1:Entity, ent2:Entity) 
    extends Dimension(id, value, loc)