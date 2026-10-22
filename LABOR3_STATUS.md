# SmartFactory – Labor 3

Das Projekt ist die eigenständige Fortsetzung des SmartFactory-Projekts im Branch `labor3`.

## Bedienung

- **Start/Stop** starten und stoppen die Bewegung der Steine auf den vier farbigen Linien sowie die Laufzeit. Stop beendet auch eine laufende Roboterbewegung.
- **Move**: Eine Richtungstaste startet fortlaufende Bewegung oder Drehung. Eine andere Taste wechselt die Richtung. Der mittlere **■**-Knopf stoppt den Roboter.
- **Step**: Jede Richtungstaste führt genau einen Schritt oder eine 5°-Drehung aus.
- **0°** richtet den Roboter wieder auf 0° aus, sofern an der Position keine Kollision entsteht.
- Ein Stein dockt automatisch an, wenn sein Mittelpunkt höchstens 10 Pixel vom Greifpunkt an der oberen Mitte des Roboters entfernt ist. **Undock** legt ihn ab und lässt den Roboter etwa 12 Pixel rückwärts fahren. Normal abgelegte Steine können danach erneut aufgehoben werden.
- Recycling- und Lieferlinie sind im Layout und in der Kollisionsprüfung enthalten. Die Verarbeitung abgelegter Steine an diesen Linien ist noch offen und soll in der Vorlesung ergänzt werden. **Order** ist editierbar; **Delivered** zeigt bis dahin 0.

Der Roboter kollidiert mit dem Fabrikrand und allen sechs Linien, auch bei Drehungen. Roboter und Steine verwenden für die Anzeige präzise Koordinaten, damit schräge Bewegungen glatt dargestellt werden.

Starten: `dotnet run --project SmartFactory.csproj`

Der lokale Branch `labor3` ist mit `origin` (`LaloMohamad/SmartFactoryApp`) verbunden. Änderungen müssen vor dem Hochladen noch committed werden.
