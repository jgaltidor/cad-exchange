package protosw;
import java.io.*;
import java.util.*;
import static java.lang.String.format;

public final class ProToSW
{
	public static void main(String[] args) {
		if(args.length < 1) {
			System.err.println(
				"usage: pro2sw <pro/e part file 1> ... <pro/e part file 1>");
			System.exit(1);
		}
		for(String filepath : args) {
			try { convertProPrt(filepath); }
			catch(Exception e) {
				System.err.println("Error while processing " + filepath);
				System.err.println(e.toString());
			}
		}
		System.out.println("Completed processing all input pro/e part files");
	}
	
	public static void convertProPrt(String pfilepath) throws Exception {
		convertProPrt(new File(pfilepath));
	}
	public static void convertProPrt(File pfile) throws Exception {
		String modelname = pathWithoutExt(pfile.getName());
		// Running proexporter
		LinkedList<String> cmdlist = new LinkedList<String>();
		cmdlist.add("runproexporter");
		cmdlist.add(pfile.toString());
		System.out.println("Running proexporter");
		int exitcode = execute(cmdlist);
		if(exitcode != 0) {
			throw new Exception(format(
				"Error code %d returned while executing command: %s",
				exitcode, joinstr(" ", cmdlist)));
		}
		// Running psconv
		String proxmldir = "pxml_" + modelname.toUpperCase();
		cmdlist.clear();
		cmdlist.add("psconv");
		cmdlist.add(proxmldir);
		System.out.println("Running psconv");
		exitcode = execute(cmdlist);
		if(exitcode != 0) {
			throw new Exception(format(
				"Error code %d returned while executing command: %s",
				exitcode, joinstr(" ", cmdlist)));
		}
		// Running swimporter
		String swxmldir = "sxml_" + proxmldir;
		String swprtfilename = "sw_" + modelname + ".sldprt";
		cmdlist.clear();
		cmdlist.add("swimporter");
		cmdlist.add(swprtfilename);
		cmdlist.add(swxmldir);
		System.out.println("Running swimporter");
		exitcode = execute(cmdlist);
		if(exitcode != 0) {
			throw new Exception(format(
				"Error code %d returned while executing command: %s",
				exitcode, joinstr(" ", cmdlist)));
		}
	}

	public static String pathWithoutExt(String filepath) {
		return pathWithoutExt(new File(filepath));
	}
	
	public static String pathWithoutExt(File file) {
		String parent = file.getParent();
		if(parent == null) parent = "";
		String basename = file.getName();
		int index = basename.indexOf('.');
		String baseNoExt = (index == -1) ?
			basename : basename.substring(0, index);
		return parent + baseNoExt;
	}

	public static int execute(List<String> cmdlist) throws InterruptedException, IOException {
		return execute(cmdlist, null);
	}

	public static int execute(List<String> incmdlist, String[] envp)
		throws InterruptedException, IOException
	{
		LinkedList<String> cmdlist = new LinkedList<String>();
		if(onWindows()) {
			cmdlist.add("cmd");
			cmdlist.add("/c");
		}
		cmdlist.addAll(incmdlist);
		System.out.println("Executing: " + joinstr(" ", cmdlist));
		ProcessBuilder builder = new ProcessBuilder(cmdlist);
		builder.redirectErrorStream(true);
		Process process = builder.start();
		System.out.println("output from command:");
		printOutStream(process.getInputStream());
		process.waitFor();
		final int exitValue = process.exitValue();
		System.out.println("Exit code from command: " + exitValue);
		System.out.println();
		return exitValue;
	}

	public static boolean onWindows()
	{
		String osName = System.getProperty("os.name");
		return osName.toLowerCase().startsWith("windows");
	}

	public static void printOutStream(InputStream istream) throws IOException
	{
			BufferedReader br = new BufferedReader(new InputStreamReader(istream));
			String line = br.readLine();
			while(line != null) {
				System.out.println(line);
				line = br.readLine();
			}
			br.close();
	}
	
	public static String joinstr(String sep, List<String> l) {
		String str = "";
		for(String s : l) str += s + sep;
		return str;
	}
}
