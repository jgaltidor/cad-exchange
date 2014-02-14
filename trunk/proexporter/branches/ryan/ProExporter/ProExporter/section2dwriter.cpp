#include "section2dwriter.h"
#include "utils.h"

// Pro/Toolkit Includes
extern "C"
{
#include "ProSecdim.h"
}

#include <cassert>
#include <string>

using namespace std;

proexporter::Section2dWriter::Section2dWriter(ProSection& section, TabPrinter& out)
	: section(section), out(out)
{}


void proexporter::Section2dWriter::exportXML() {
	out.printlnf("<pro2dsection name=\"%s\">", getSectionName(section).c_str());
	out.incrementTabs();
	this->writeEntities();
	this->writeConstraints();
	this->writeDimensions();
	out.decrementTabs();
	out.printlnf("</pro2dsection>");
}

ProError proexporter::Section2dWriter::writeEntities() {
	ProIntlist ent_ids;
	int n_ids;
	ProError status = ProSectionEntityIdsGet(section, &ent_ids, &n_ids);
	errchk("ProSectionEntityIdsGets()",
		"proexporter::Section2dWriter::writeEntities()", status);
	if(status != PRO_TK_NO_ERROR) {
		ProTKFprintf(stderr, "Not able to access section entity information \n" );
		return status;
	}
	out.printlnf("<pro2dEntities>");
	out.incrementTabs();
    for (int i = 0; i < n_ids; i++) {
		this->writeEntity(ent_ids[i]);
	}
	out.decrementTabs();
	out.printlnf("</pro2dEntities>");
    status = ProArrayFree((ProArray*) &ent_ids);
    errchk("ProArrayFree()", "proexporter::Section2dWriter::writeEntities()", status);
	return status;
}


ProError proexporter::Section2dWriter::writeEntity(int entityId) {
	Pro2dEntdef* p_ent;
	// Get Entity by entityId
	ProError status = ProSectionEntityGet(section, entityId, &p_ent);
	errchk("ProSectionEntityGet()", "writeEntity()", status);
	if(status != PRO_TK_NO_ERROR) return status;

	// Is entity a projection
	ProBoolean is_projection;
    status = ProSectionEntityIsProjection(section, entityId,
		&is_projection);
    errchk("ProSectionEntityIsProjection()", "proexporter::Section2dWriter::writeStartTag()",
		status);

	std::string isProjectionStr = getBoolStr(is_projection);
	std::string typeStr = getTypeStr(p_ent->type);
	
	// Write begin tag
	out.printlnf("<pro2dEntity id=\"%d\" isProjection=\"%s\" type=\"%s\" >",
		entityId, isProjectionStr.c_str(), typeStr.c_str());
	out.incrementTabs();

	switch(p_ent->type) {
		case PRO_2D_POINT:
			this->writePro2dPointdef((Pro2dPointdef*) p_ent);
			break;
		case PRO_2D_COORD_SYS:
			this->writePro2dCoordSysdef((Pro2dCoordSysdef*) p_ent);
			break;
		case PRO_2D_LINE:
			this->writePro2dLinedef((Pro2dLinedef*) p_ent);
			break;
		case PRO_2D_CENTER_LINE:
			this->writePro2dClinedef((Pro2dClinedef*) p_ent);
			break;
		case PRO_2D_ARC:
			this->writePro2dArcdef((Pro2dArcdef*) p_ent);
			break;
		case PRO_2D_CIRCLE:
		case PRO_2D_CONSTR_CIRCLE:
			this->writePro2dCircledef((Pro2dCircledef*) p_ent);
			break;
		case PRO_2D_POLYLINE:
			this->writePro2dPolylinedef((Pro2dPolylinedef*) p_ent);
			break;
		case PRO_2D_SPLINE:
			this->writePro2dSplinedef((Pro2dSplinedef*) p_ent);
			break;
		case PRO_2D_TEXT:
			this->writePro2dTextdef((Pro2dTextdef*) p_ent);
			break;
		case PRO_2D_BLEND_VERTEX:
			this->writePro2dBlendVertexdef((Pro2dBlendVertexdef*) p_ent);
			break;
		case PRO_2D_ELLIPSE:
			writePro2dEllipsedef((Pro2dEllipsedef*) p_ent);
			break;
		case PRO_2D_CONIC:
			this->writePro2dConicdef((Pro2dConicdef*) p_ent);
			break;
		default:
			assert("UNKNOWN Entity Type" == 0);
			break;
	}
	out.decrementTabs();
	// Write end tag
	out.printlnf("</pro2dEntity>");
	return status;
}

/**
<pro2dEntity id="7" isProjection="true" type="PRO_2D_POINT">
  <pnt>
    <Pro2dPnt x="3.5" y="4.7" />
  </pnt>
</pro2dEntity>
  */
void proexporter::Section2dWriter::writePro2dPointdef(Pro2dPointdef* p_pnt) {
	out.printlnf("<pnt>");
	out.incrementTabs();
		this->writePro2dPnt(p_pnt->pnt);
		out.decrementTabs();
	out.printlnf("</pnt>");
}


void proexporter::Section2dWriter::writePro2dCoordSysdef(Pro2dCoordSysdef* p_coord) {
	this->writePro2dPointdef(p_coord);
}


/**
<pro2dEntity id ="7" isProjection="true" type="PRO_2D_LINE">
  <end1>
    <Pro2dPnt x="3.5" y="4.7" />
  </end1>
  <end2>
    <Pro2dPnt x="3.5" y="4.7" />
  </end2>
</pro2dEntity>
  */
void proexporter::Section2dWriter::writePro2dLinedef(Pro2dLinedef* p_line) {
	out.printlnf("<end1>");
	out.incrementTabs();
		this->writePro2dPnt(p_line->end1);
		out.decrementTabs();
	out.printlnf("</end1>");
	out.printlnf("<end2>");
	out.incrementTabs();
		this->writePro2dPnt(p_line->end2);
		out.decrementTabs();
	out.printlnf("</end2>");
}

void proexporter::Section2dWriter::writePro2dClinedef(Pro2dClinedef* p_cline) {
	writePro2dLinedef(p_cline);
}

/**
<pro2dEntity id ="7" isProjection="true" type="PRO_2D_ARC">
  <center>
    <Pro2dPnt x="3.5" y="4.7" />
  </center>
  <start_angle>
    <ProAngle radians="1.5" />
  </start_angle>
  <end_angle>
    <ProAngle radians="1.5" />
  </end_angle>
  <radius>5</radius>
</pro2dEntity>
  */
void proexporter::Section2dWriter::writePro2dArcdef(Pro2dArcdef* p_arc) {
	out.printlnf("<center>");
	out.incrementTabs();
		this->writePro2dPnt(p_arc->center);
		out.decrementTabs();
	out.printlnf("</center>");
	out.printlnf("<start_angle>");
	out.incrementTabs();
		this->writeProAngle(p_arc->start_angle);
		out.decrementTabs();
	out.printlnf("</start_angle>");
	out.printlnf("<end_angle>");
	out.incrementTabs();
		this->writeProAngle(p_arc->end_angle);
		out.decrementTabs();
	out.printlnf("</end_angle>");
	out.printlnf("<radius>%4.2lf</radius>", p_arc->radius);
}


/**
<pro2dEntity id ="7" isProjection="true" type="PRO_2D_CIRCLE">
  <center>
    <Pro2dPnt x="3.5" y="4.7" />
  </center>
  <radius>5</radius>
</pro2dEntity>
  */
void proexporter::Section2dWriter::writePro2dCircledef(Pro2dCircledef* p_circle) {
	out.printlnf("<center>");
	out.incrementTabs();
		this->writePro2dPnt(p_circle->center);
		out.decrementTabs();
	out.printlnf("</center>");
	out.printlnf("<radius>%4.2lf</radius>", p_circle->radius);
}


/**
<pro2dEntity id ="7" isProjection="true" type="PRO_2D_POLYLINE">
  <n_points>3</n_points>
  <point_arr>
    <Pro2dPnt x="3.5" y="4.7" />
    <Pro2dPnt x="2.5" y="4.7" />
    <Pro2dPnt x="6.5" y="4.7" />
  </point_arr>
</pro2dEntity>
  */
void proexporter::Section2dWriter::writePro2dPolylinedef(Pro2dPolylinedef* p_polyline) {
	const unsigned int n_points = p_polyline->n_points;
	this->writeNPoints(n_points);
	out.printlnf("<point_arr>");
	out.incrementTabs();
		for(int i = 0; i < n_points; i++) {
			this->writePro2dPnt(p_polyline->point_arr[i]);
		}
		out.decrementTabs();
	out.printlnf("</point_arr>");
}

/**
<pro2dEntity id ="7" isProjection="true" type="PRO_2D_SPLINE">
  <n_points>3</n_points>
  <point_arr>
    <Pro2dPnt x="3.5" y="4.7" />
    <Pro2dPnt x="2.5" y="4.7" />
    <Pro2dPnt x="6.5" y="4.7" />
  </point_arr>
  <start_tang_angle>
    <ProAngle radians="1.5" />
  </start_tang_angle>
  <end_tang_angle>
    <ProAngle radians="1.5" />
  </end_tang_angle>
</pro2dEntity>
 */
void proexporter::Section2dWriter::writePro2dSplinedef(Pro2dSplinedef* p_spline) {
	const unsigned int n_points = p_spline->n_points;
	writeNPoints(n_points);
	out.printlnf("<point_arr>");
	out.incrementTabs();
		for(int i = 0; i < n_points; i++) {
			this->writePro2dPnt(p_spline->point_arr[i]);
		}
		out.decrementTabs();
	out.printlnf("</point_arr>");
	out.printlnf("<start_tang_angle>");
	out.incrementTabs();
		this->writeProAngle(p_spline->start_tang_angle);
		out.decrementTabs();
	out.printlnf("</start_tang_angle>");
	out.printlnf("<end_tang_angle>");
	out.incrementTabs();
		this->writeProAngle(p_spline->end_tang_angle);
		out.decrementTabs();
	out.printlnf("</end_tang_angle>");
}


/**
<pro2dEntity id ="7" isProjection="true" type="PRO_2D_TEXT">
  <first_corner>
    <Pro2dPnt x="3.5" y="4.7" />
  </first_corner>
  <second_corner>
    <Pro2dPnt x="2.5" y="4.7" />
  </second_corner>
  <string>
    <ProComment>Some comment</ProComment>
  </string>
  <font_name>
    <ProComment>Bold</ProComment>
  </font_name>
</pro2dEntity>
*/
void proexporter::Section2dWriter::writePro2dTextdef(Pro2dTextdef* p_text) {
	out.printlnf("<first_corner>");
	out.incrementTabs();
		this->writePro2dPnt(p_text->first_corner);
		out.decrementTabs();
	out.printlnf("</first_corner>");
	out.printlnf("<second_corner>");
	out.incrementTabs();
		this->writePro2dPnt(p_text->second_corner);
		out.decrementTabs();
	out.printlnf("</second_corner>");
	out.printlnf("<string>");
	out.incrementTabs();
		this->writeComment(p_text->string);
		out.decrementTabs();
	out.printlnf("</string>");
	out.printlnf("<font_name>");
	out.incrementTabs();
		this->writeComment(p_text->font_name);
		out.decrementTabs();
	out.printlnf("</font_name>");
}


/**
<pro2dEntity id ="7" isProjection="true" type="PRO_2D_BLEND_VERTEX">
  <pnt>
    <Pro2dPnt x="3.5" y="4.7" />
  </pnt>
  <depth_level>3</depth_level>
</pro2dEntity>
*/
void proexporter::Section2dWriter::writePro2dBlendVertexdef(Pro2dBlendVertexdef* p_bvertex) {
	out.printlnf("<pnt>");
	out.incrementTabs();
		this->writePro2dPnt(p_bvertex->pnt);
		out.decrementTabs();
	out.printlnf("</pnt>");
	out.printlnf("<depth_level>%d</depth_level>", p_bvertex->depth_level);
}


/**
<pro2dEntity id ="7" isProjection="true" type="PRO_2D_ELLIPSE">
  <origin>
    <Pro2dPnt x="3.5" y="4.7" />
  </origin>
  <x_radius>2</x_radius>
  <y_radius>3</y_radius>
</pro2dEntity>
*/
void proexporter::Section2dWriter::writePro2dEllipsedef(Pro2dEllipsedef* p_ellipse) {
	out.printlnf("<origin>");
	out.incrementTabs();
		this->writePro2dPnt(p_ellipse->origin);
		out.decrementTabs();
	out.printlnf("</origin>");
	out.printlnf("<x_radius>%4.2lf</x_radius>", p_ellipse->x_radius);
	out.printlnf("<y_radius>%4.2lf</y_radius>", p_ellipse->y_radius);
}


/**
<pro2dEntity id ="7" isProjection="true" type="PRO_2D_CONIC">
  <first_end_point>
    <Pro2dPnt x="3.5" y="4.7" />
  </first_end_point>
  <second_end_point>
    <Pro2dPnt x="3.5" y="4.7" />
  </second_end_point>
  <shoulder_point>
    <Pro2dPnt x="3.5" y="4.7" />
  </shoulder_point>
  <parameter>2</parameter>
</pro2dEntity>
*/
void proexporter::Section2dWriter::writePro2dConicdef(Pro2dConicdef* p_conic) {
	out.printlnf("<first_end_point>");
	out.incrementTabs();
		this->writePro2dPnt(p_conic->first_end_point);
		out.decrementTabs();
	out.printlnf("</first_end_point>");
	out.printlnf("<second_end_point>");
	out.incrementTabs();
		this->writePro2dPnt(p_conic->second_end_point);
		out.decrementTabs();
	out.printlnf("</second_end_point>");
	out.printlnf("<shoulder_point>");
	out.incrementTabs();
		this->writePro2dPnt(p_conic->shoulder_point);
		out.decrementTabs();
	out.printlnf("</shoulder_point>");
	out.printlnf("<parameter>%4.2lf</parameter>", p_conic->parameter);
}


/**
  * <Pro2dPnt x="3.5" y="4.7" />
  */
void proexporter::Section2dWriter::writePro2dPnt(Pro2dPnt& pnt) {
	out.printlnf("<Pro2dPnt x=\"%4.2lf\" y=\"%4.2lf\" />", pnt[0], pnt[1]);
}


/**
  * <ProAngle radians="1.5" />
  */
void proexporter::Section2dWriter::writeProAngle(ProAngle& angle) {
	out.printlnf("<ProAngle radians=\"%4.2lf\" />", angle);
}


void proexporter::Section2dWriter::writeNPoints(unsigned int n_points) {
	out.printlnf("<n_points>%d</n_points>", n_points);
}


void proexporter::Section2dWriter::writeComment(ProComment& comment) {
	if(out.isStartingNewLine()) out.printTabs();
	FILE* fp = out.getFileDescriptor();
	ProTKFprintf(fp, "<ProComment>%s</ProComment>", comment);
	out.println();
}


ProError proexporter::Section2dWriter::writeConstraints() {
	ProIntlist con_ids;
	int n_ids;
	ProError status = ProSectionConstraintsIdsGet(section, &con_ids, &n_ids);
	errchk("ProSectionConstraintsIdsGet()",
		"proexporter::Section2dWriter::writeConstraints()", status);
	if(status != PRO_TK_NO_ERROR) {
		ProTKFprintf(stderr, "Not able to access section constraint information \n");
		return status;
	}
	
	out.printlnf("<pro2dConstraints>");
	out.incrementTabs();
    for (int i = 0; i < n_ids; i++) {
		this->writeConstraint(con_ids[i]);
	}
	out.decrementTabs();
	out.printlnf("</pro2dConstraints>");

    status = ProArrayFree((ProArray*) &con_ids);
    errchk("ProArrayFree()", "proexporter::Section2dWriter::writeConstraints()", status);
	return status;
}


ProError proexporter::Section2dWriter::writeConstraint(int constraintId) {
    ProConstraintType con_type;
    ProConstraintStatus con_status;
	int n_ref_ids;
	int* ref_ids;
	ProSectionPointType* p_types;

	ProError status = ProSectionConstraintsGet(section, constraintId,
		&con_type, &con_status, &n_ref_ids, &ref_ids, &p_types);
	errchk("ProSectionConstraintsGet()",
		"proexporter::Section2dWriter::writeConstraint()", status);
	if(status != PRO_TK_NO_ERROR) return status;

	string typeStr = getTypeStr(con_type);
	string statusStr = getStatusStr(con_status);
	
	// Write start tag
	out.printlnf("<pro2dConstraint id=\"%d\" type=\"%s\" status=\"%s\">",
		constraintId, typeStr.c_str(), statusStr.c_str());
	out.incrementTabs();
	this->writeEntityReferences(ref_ids, n_ref_ids);
	this->writePointTypes(p_types, n_ref_ids);
	out.decrementTabs();
	// Write end tag
	out.printlnf("</pro2dConstraint>");

    status = ProArrayFree((ProArray*) &ref_ids);
    errchk("ProArrayFree()", "proexporter::Section2dWriter::writeConstraint()", status);
    status = ProArrayFree((ProArray*) &p_types);
    errchk("ProArrayFree()", "proexporter::Section2dWriter::writeConstraint()", status);
	return status;
}


ProError proexporter::Section2dWriter::writeDimensions() {
	ProIntlist dim_ids;
	int n_dimids;
	ProError status = ProSecdimIdsGet(section, &dim_ids, &n_dimids);
	errchk("ProSecdimIdsGet()",
		"proexporter::Section2dWriter::writeDimensions()", status);
	if(status != PRO_TK_NO_ERROR) {
		ProTKFprintf(stderr, "Not able to access section dimension information \n");
		return status;
	}
	
	out.printlnf("<pro2dDimensions>");
	out.incrementTabs();
    for (int i = 0; i < n_dimids; i++) {
		this->writeDimension(dim_ids[i]);
	}
	out.decrementTabs();
	out.printlnf("</pro2dDimensions>");

    status = ProArrayFree((ProArray*) &dim_ids);
    errchk("ProArrayFree()", "proexporter::Section2dWriter::writeDimensions()", status);
	return status;
}


ProError proexporter::Section2dWriter::writeDimension(int dimensionId) {
	double value;
	ProError status = ProSecdimValueGet(section, dimensionId, &value);
	errchk("ProSectionConstraintsGet()",
		"proexporter::Section2dWriter::writeConstraint()", status);
	if(status != PRO_TK_NO_ERROR) return status;
	
	ProSecdimType dim_type;
	status = ProSecdimTypeGet(section, dimensionId, &dim_type);
	errchk("ProSectionConstraintsGet()",
		"proexporter::Section2dWriter::writeConstraint()", status);
	if(status != PRO_TK_NO_ERROR) return status;
	
	string typeStr = getTypeStr(dim_type);

	int n_ref_ids;
	int* ref_ids;
	ProSectionPointType* p_types;
	status = ProSecdimReferencesGet(section, dimensionId, &ref_ids,
		&p_types, &n_ref_ids);
	errchk("ProSecdimReferencesGet()",
		"proexporter::Section2dWriter::writeDimension()", status);
	if(status != PRO_TK_NO_ERROR) return status;

	Pro2dPnt location;
	status = ProSecdimLocationGet(section, dimensionId, location);
	errchk("ProSecdimLocationGet()", 
		"proexporter::Section2dWriter::writeDimension()", status);
	if(status != PRO_TK_NO_ERROR) return status;

	// Write start tag
	out.printlnf("<pro2dDimension id=\"%d\" type=\"%s\" value=\"%4.2lf\" >",
		dimensionId, typeStr.c_str(), value);
	out.incrementTabs();
	this->writeEntityReferences(ref_ids, n_ref_ids);
	this->writePointTypes(p_types, n_ref_ids);
	this->writeLocation(location);
	out.decrementTabs();
	// Write end tag
	out.printlnf("</pro2dDimension>");

    status = ProArrayFree((ProArray*) &ref_ids);
    errchk("ProArrayFree()", "proexporter::Section2dWriter::writeDimension()", status);
    status = ProArrayFree((ProArray*) &p_types);
    errchk("ProArrayFree()", "proexporter::Section2dWriter::writeDimension()", status);
	return status;
}

void proexporter::Section2dWriter::writeLocation(Pro2dPnt& location) {
	out.printlnf("<location>");
	out.incrementTabs();
	writePro2dPnt(location);
	out.decrementTabs();
	out.printlnf("</location>");
}

void proexporter::Section2dWriter::writeEntityReferences(const int* ref_ids, const int n_ref_ids) {
	out.printlnf("<entityReferences>");
	out.incrementTabs();
	for(int i = 0; i < n_ref_ids; i++) {
		this->writeEntityReference(ref_ids[i]);
	}
	out.decrementTabs();
	out.printlnf("</entityReferences>");
}


void proexporter::Section2dWriter::writeEntityReference(const int ref_id) {
	out.printlnf("<entityReference id =\"%d\" />", ref_id);
}


void proexporter::Section2dWriter::writePointTypes(const ProSectionPointType* ref_ids, const int n_ref_ids) {
	out.printlnf("<pointTypes>");
	out.incrementTabs();
	for(int i = 0; i < n_ref_ids; i++) {
		this->writePointType(ref_ids[i]);
	}
	out.decrementTabs();
	out.printlnf("</pointTypes>");
}

void proexporter::Section2dWriter::writePointType(const ProSectionPointType& p_type) {
	out.printlnf("<pointType type =\"%s\" />", getTypeStr(p_type).c_str());
}

