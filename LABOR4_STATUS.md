# SmartFactory – Labor 4

## Umgesetzt

- Der Roboter nimmt Steine von Produktions- und Recyclinglinien auf, aber nicht von der Delivery-Line.
- Undock ist nur an der Delivery-Line möglich und zeigt Erfolg oder den Fehlergrund an.
- Bei neuen Steinen ist jeder zweite defekt. Gute Steine fahren rechts zum Kunden, defekte links zum Recycling.
- Die Lieferung erhöht den Zähler der Farbe und erzeugt auf der Ursprungslinie einen neuen Stein.
- Die Recyclinglinie hat vier Plätze. Beim Ankommen entscheidet ein Zufallstest mit 50 % Erfolg über die Reparatur. Nach einem Fehlschlag kann derselbe Stein erneut zum Recycling gebracht werden. Bei voller Linie wartet ein weiterer Stein am linken Ende der Delivery-Line.
- Move/Step, Rotation, Kollisionen und Start/Stop aus Labor 3 bleiben erhalten.

## Offene Vorgaben

Das PDF verweist auf ein Video und bezeichnet seine Aufgabenliste ausdrücklich als unvollständig. Das Video lag hier nicht unter dem angegebenen Namen vor. Die Order-Felder bleiben editierbare Zielzahlen; das PDF legt nicht fest, wie sie den Transport steuern. Die genaue Anzeige und das Timing der Recycling- und Kundenwege können daher noch vom Video abweichen.

Starten: `dotnet run --project SmartFactory.csproj`
