rmdir /s /q apidoc
mkdir apidoc
scaladoc -d apidoc *.scala *.java
