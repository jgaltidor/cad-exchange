package psconv.util

case class Point2D(x:Double, y:Double)

case class Point3D(x:Double, y:Double, z:Double)

case class Line2D(p1:Point2D, p2:Point2D) {
	if(p1 == p2)
		throw new IllegalArgumentException("p1 and p2 must be distinct points")
	
	lazy val slope = (p2.y - p1.y) / (p2.x - p1.x)
	
	lazy val yintercept = p1.y - p1.x*slope
	
	def getY(x:Double) = slope*x + yintercept
	
	def getX(y:Double) = (y - yintercept) / slope

	lazy val isVertical = p1.x == p2.x
	
	lazy val isHorizontal = p1.y == p2.y
}


object MathFun
{
	def intersection(line1:Line2D, line2:Line2D):Option[Point2D] = {
		if(line1.isVertical) {
			if(!line2.isVertical) {
				val x = line1.p1.x
				Some(Point2D(x, line2.getY(x)))
			}
			else None
		}
		else if(line2.isVertical) {
			if(!line1.isVertical) {
				val x = line2.p1.x
				Some(Point2D(x, line1.getY(x)))
			}
			else None
		}
		else {
			val m1 = line1.slope
			val m2 = line2.slope
			if(Math.abs(m1 - m2) < 0.0001) // if slopes are the same 
				None
			else {
				val b1 = line1.yintercept
				val b2 = line2.yintercept
				val x = (b2 - b1)/(m1 - m2)
				val y = x*m1+b1
				Some(Point2D(x, y))
			}
		}
	}
	
	def isXAxis(line:Line2D):Boolean =
		(line.p1 == Point2D(0, 0) &&
		line.p2.x != 0 && line.p2.y == 0)

	def isYAxis(line:Line2D):Boolean =
		(line.p1 == Point2D(0, 0) &&
		line.p2.y != 0 && line.p2.x == 0)
}


class ListMax[A <% Ordered[A]]
{
	import scala.collection.JavaConversions._
    val list = new java.util.LinkedList[Ordered[A]]
	private var maxElem:Option[A] = None
	
	def ++=(itr:Iterable[A]):Unit =
		itr.foreach(add(_))
	
	def add(elem:A):Unit = {
		maxElem match {
			case Some(maxObj) =>
				if(maxObj < elem) maxElem = Some(elem)
			case None =>
				maxElem = Some(elem)
		}
		list add elem
	}

	def max:A = maxElem match {
		case Some(elem) => elem
		case None => throw new java.util.NoSuchElementException
	}
	
	def isEmpty = list.isEmpty	
}


trait TabPrintWrapper {
	protected var out:TabPrintStream = null

	def print(obj:Any) = out.print(obj)
	def printf(text:String, args:Object*) = out.printf(text, args: _*)
	def println() = out.println
	def println(x:Any) = out.println(x)
	def incrementTabs = out.incrementTabs
	def decrementTabs = out.decrementTabs
	def close = out.close
}

