#include "utils.h"
#include <cassert>

extern "C"
{
#include "ProUtil.h"
}

using namespace std;

/** Implementation copied based on ERROR_CHECK macro from
  * $PROTOOLKIT/protk_appls/includes/user_tk_error.h
  */
void proexporter::errchk(const char* func, const char* call, const ProError err) {
	if ( err != PRO_TK_NO_ERROR ) {
		fprintf(stderr, "%s in %s returned %d\n", call, func, err);
		//cerr << call << " in " << func << " returned " << err << endl;
		// exit(1);
	}
}

string proexporter::getSectionName(const ProSection& section) {
    ProName wname;
    char name[PRO_NAME_SIZE];

    ProError status = ProSectionNameGet(section, wname);
    errchk("ProSectionNameGet()", "getSectionName()", status);
    ProWstringToString(name, wname);
	return name;
}

string proexporter::getBoolStr(const ProBoolean& probool) {
	return (probool == PRO_B_TRUE) ? "true" : "false"; 
}

string proexporter::getTypeStr(const Pro2dEntType& type) {
	switch(type) {
		case PRO_2D_POINT:         return "PRO_2D_POINT";
		case PRO_2D_COORD_SYS:     return "PRO_2D_COORD_SYS";
		case PRO_2D_LINE:          return "PRO_2D_LINE";
		case PRO_2D_CENTER_LINE:   return "PRO_2D_CENTER_LINE";
		case PRO_2D_ARC:           return "PRO_2D_ARC";
		case PRO_2D_CIRCLE:        return "PRO_2D_CIRCLE";
		case PRO_2D_CONSTR_CIRCLE: return "PRO_2D_CONSTR_CIRCLE";
		case PRO_2D_POLYLINE:      return "PRO_2D_POLYLINE";
		case PRO_2D_SPLINE:        return "PRO_2D_SPLINE";
		case PRO_2D_TEXT:          return "PRO_2D_TEXT";
		case PRO_2D_BLEND_VERTEX:  return "PRO_2D_BLEND_VERTEX";
		case PRO_2D_ELLIPSE:       return "PRO_2D_ELLIPSE";
		case PRO_2D_CONIC:         return "PRO_2D_CONIC";
		default:
			assert("UNKNOWN Entity Type" == 0);
	}
	return "";
}

string proexporter::getTypeStr(const ProConstraintType& type) {
	switch(type) {
		case PRO_CONSTRAINT_TYPE_UNKNOWN:     return "PRO_CONSTRAINT_TYPE_UNKNOWN";
		case PRO_CONSTRAINT_SAME_POINT:       return "PRO_CONSTRAINT_SAME_POINT";
		case PRO_CONSTRAINT_HORIZONTAL_ENT:   return "PRO_CONSTRAINT_HORIZONTAL_ENT";
		case PRO_CONSTRAINT_VERTICAL_ENT:     return "PRO_CONSTRAINT_VERTICAL_ENT";
		case PRO_CONSTRAINT_PNT_ON_ENT:       return "PRO_CONSTRAINT_PNT_ON_ENT";
		case PRO_CONSTRAINT_TANGENT_ENTS:     return "PRO_CONSTRAINT_TANGENT_ENTS";
		case PRO_CONSTRAINT_ORTHOG_ENTS:      return "PRO_CONSTRAINT_ORTHOG_ENTS";
		case PRO_CONSTRAINT_EQUAL_RADII:      return "PRO_CONSTRAINT_EQUAL_RADII";
		case PRO_CONSTRAINT_PARALLEL_ENTS:    return "PRO_CONSTRAINT_PARALLEL_ENTS";
		case PRO_CONSTRAINT_EQUAL_SEGMENTS:   return "PRO_CONSTRAINT_EQUAL_SEGMENTS";
		case PRO_CONSTRAINT_COLLINEAR_LINES:  return "PRO_CONSTRAINT_COLLINEAR_LINES";
		case PRO_CONSTRAINT_SYMMETRY:         return "PRO_CONSTRAINT_SYMMETRY";
		case PRO_CONSTRAINT_SAME_COORD:       return "PRO_CONSTRAINT_SAME_COORD";
		case PRO_CONSTRAINT_SAME_Y_COORD:     return "PRO_CONSTRAINT_SAME_Y_COORD";
		case PRO_CONSTRAINT_SAME_X_COORD:     return "PRO_CONSTRAINT_SAME_X_COORD";
		case PRO_CONSTRAINT_MIDDLE_POINT:     return "PRO_CONSTRAINT_MIDDLE_POINT";
		case PRO_CONSTRAINT_EQUAL_CURVATURE:  return "PRO_CONSTRAINT_EQUAL_CURVATURE";
		default:
			assert("UNKNOWN Constraint Type" == 0);
	}
	return "";
}

string proexporter::getStatusStr(const ProConstraintStatus& status) {
	switch(status) {
		case PRO_TK_CONSTRAINT_DENIED:  return "PRO_TK_CONSTRAINT_DENIED";
		case PRO_TK_CONSTRAINT_ENABLED: return "PRO_TK_CONSTRAINT_ENABLED";
		default:
			assert("UNKNOWN Constraint Status" == 0);
	}
	return "";
}

string proexporter::getTypeStr(const ProSecdimType& status) {
	switch(status) {
		case PRO_TK_DIM_TYPE_UNKNOWN:      return "PRO_TK_DIM_TYPE_UNKNOWN";
		case PRO_TK_DIM_NONE:              return "PRO_TK_DIM_NONE";
		case PRO_TK_DIM_LINE:              return "PRO_TK_DIM_LINE";
		case PRO_TK_DIM_LINE_POINT:        return "PRO_TK_DIM_LINE_POINT";
		case PRO_TK_DIM_RAD:               return "PRO_TK_DIM_RAD";
		case PRO_TK_DIM_DIA:               return "PRO_TK_DIM_DIA";
		case PRO_TK_DIM_LINE_LINE:         return "PRO_TK_DIM_LINE_LINE";
		case PRO_TK_DIM_PNT_PNT:           return "PRO_TK_DIM_PNT_PNT";
		case PRO_TK_DIM_PNT_PNT_HORIZ:     return "PRO_TK_DIM_PNT_PNT_HORIZ";
		case PRO_TK_DIM_PNT_PNT_VERT:      return "PRO_TK_DIM_PNT_PNT_VERT";
		case PRO_TK_DIM_AOC_AOC_TAN_HORIZ: return "PRO_TK_DIM_AOC_AOC_TAN_HORIZ";
		case PRO_TK_DIM_AOC_AOC_TAN_VERT:  return "PRO_TK_DIM_AOC_AOC_TAN_VERT";
		case PRO_TK_DIM_ARC_ANGLE:         return "PRO_TK_DIM_ARC_ANGLE";
		case PRO_TK_DIM_LINES_ANGLE:       return "PRO_TK_DIM_LINES_ANGLE";
		case PRO_TK_DIM_LINE_AOC:          return "PRO_TK_DIM_LINE_AOC";
		case PRO_TK_DIM_LINE_CURVE_ANGLE:  return "PRO_TK_DIM_LINE_CURVE_ANGLE";
		case PRO_TK_DIM_CONIC_PARAM:       return "PRO_TK_DIM_CONIC_PARAM";
		case PRO_TK_DIM_EXT_APP:           return "PRO_TK_DIM_EXT_APP";
		case PRO_TK_DIM_LIN_MULTI_OFFSET:  return "PRO_TK_DIM_LIN_MULTI_OFFSET";
		case PRO_TK_DIM_PNT_OFFSET:        return "PRO_TK_DIM_PNT_OFFSET";
		case PRO_TK_DIM_ELLIPSE_X_RADIUS:  return "PRO_TK_DIM_ELLIPSE_X_RADIUS";
		case PRO_TK_DIM_ELLIPSE_Y_RADIUS:  return "PRO_TK_DIM_ELLIPSE_Y_RADIUS";
		default:
			assert("UNKNOWN Dimension Type" == 0);
	}
	return "";
}

string proexporter::getTypeStr(const ProSectionPointType& type) {
	switch(type) {
		case PRO_ENT_WHOLE:           return "PRO_ENT_WHOLE";
		case PRO_ENT_START:           return "PRO_ENT_START";
		case PRO_ENT_END:             return "PRO_ENT_END";
		case PRO_ENT_CENTER:          return "PRO_ENT_CENTER";
		case PRO_ENT_LEFT_TANGENT:    return "PRO_ENT_LEFT_TANGENT";
		case PRO_ENT_RIGHT_TANGENT:   return "PRO_ENT_RIGHT_TANGENT";
		case PRO_ENT_TOP_TANGENT:     return "PRO_ENT_TOP_TANGENT";
		case PRO_ENT_BOTTOM_TANGENT:  return "PRO_ENT_BOTTOM_TANGENT";
		default:
			assert("UNKNOWN Section Point Type" == 0);
	}
	return "";
}

string proexporter::filebasename(string filepath) {
	int pos = filepath.find_last_of('\\');
	return (pos != string::npos && pos+1 < filepath.length()) ?
		filepath.substr(pos+1) : filepath;
}

string proexporter::withOutExt(string filepath) {
	int pos = filepath.find_last_of('.');
	return (pos != string::npos) ?
		filepath.substr(0, pos) : "";
}

string proexporter::toUpperCase(string s) {
	string upperStr = "";
	for(string::iterator itr = s.begin(); itr != s.end(); itr++) {
		upperStr += toupper(*itr);
	}
	return upperStr;
}

string proexporter::getPartName(string partfilepath) {
	string basename = filebasename(partfilepath);
	return toUpperCase(withOutExt(basename));
}

