javac -d . TabPrintStream.java
call scalac.bat *.scala
rem Build jar file
jar cvfm lib\psconv.jar Manifest.txt psconv
