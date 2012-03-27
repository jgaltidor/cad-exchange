javac -d . TabPrintStream.java
scalac *.scala
# Build jar file
jar cvfm lib\psconv.jar Manifest.txt psconv
