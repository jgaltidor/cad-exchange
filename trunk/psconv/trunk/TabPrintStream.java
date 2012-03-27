package psconv.util;
import java.io.*;
import java.util.*;

/**
 * To print new lines always use a println method.
 * Do not use \n to print a new line.
 * @author jaltidor
 *
 */
public class TabPrintStream extends PrintStream
{
    private String tab;
    boolean printTabs;
    private int numOfTabs = 0;

    public TabPrintStream(String filename) throws FileNotFoundException
    {
        this(filename, "    ", true);
    }

    public TabPrintStream(String filename, String tab) throws FileNotFoundException
    {
        this(filename, tab, true);
    }

    public TabPrintStream(String filename, String tab, boolean printTabs)
        throws FileNotFoundException
    {
        this(new File(filename), tab, printTabs);
    }

    public TabPrintStream(File file) throws FileNotFoundException
    {
        this(file, "    ", true);
    }

    public TabPrintStream(File file, String tab) throws FileNotFoundException
    {
        this(file, tab, true);
    }

    public TabPrintStream(File file, String tab, boolean printTabs)
        throws FileNotFoundException
    {
        super(file);
        this.tab = tab;
        this.printTabs = printTabs;
    }

    public void incrementTabs() { numOfTabs++; }

    public void decrementTabs() {
        if(numOfTabs > 0) numOfTabs--;
    }

    private final void printTabs() {
        for(int i = 0; i < numOfTabs; i++)
            super.print(tab);
        printTabs = false;
    }

    // ============================================
    // print
    // ============================================
    
    public void print(boolean b) {
        if(printTabs) printTabs();
        super.print(b);
    }

    public void print(char c) {
        if(printTabs) printTabs();
        super.print(c);
    }

    public void print(char[] s) {
        if(printTabs) printTabs();
        super.print(s);
    }

    public void print(double d) {
        if(printTabs) printTabs();
        super.print(d);
    }

    public void print(float f) {
        if(printTabs) printTabs();
        super.print(f);
    }

    public void print(int i) {
        if(printTabs) printTabs();
        super.print(i);
    }

    public void print(long l) {
        if(printTabs) printTabs();
        super.print(l);
    }

    public void print(Object obj) {
        if(printTabs) printTabs();
        super.print(obj);
    }

    public void print(String s) {
        if(printTabs) printTabs();
        super.print(s);
    }

    // ============================================
    // printf
    // ============================================
    public PrintStream printf(Locale l, String format, Object... args) {
        if(printTabs) printTabs();
        return super.printf(l, format, args);
    }

    public PrintStream printf(String format, Object... args) {
        if(printTabs) printTabs();
        return super.printf(format, args);
    }


    // ============================================
    // println
    // ============================================
    
    public void println() {
        super.println();
        printTabs = true;
    }

    public void println(boolean x) {
    	if(printTabs) printTabs();
        super.println(x);
        printTabs = true;
    }

    public void println(char x) {
    	if(printTabs) printTabs();
        super.println(x);
        printTabs = true;
    }

    public void println(char[] x) {
    	if(printTabs) printTabs();
        super.println(x);
        printTabs = true;
    }

    public void println(double x) {
    	if(printTabs) printTabs();
        super.println(x);
        printTabs = true;
    }

    public void println(float x) {
    	if(printTabs) printTabs();
        super.println(x);
        printTabs = true;
    }

    public void println(int x) {
    	if(printTabs) printTabs();
        super.println(x);
        printTabs = true;
    }

    public void println(long x) {
    	if(printTabs) printTabs();
        super.println(x);
        printTabs = true;
    }

    public void println(Object x) {
    	if(printTabs) printTabs();
        super.println(x);
        printTabs = true;
    }

    public void println(String x) {
    	if(printTabs) printTabs();
        super.println(x);
        printTabs = true;
    }
}
