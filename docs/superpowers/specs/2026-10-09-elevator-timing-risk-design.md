# One More Floor: Aufzug, Timing und Risiko

Vom Nutzer am 9. Oktober gewählte Richtung. Dieser Entwurf ersetzt das bisherige freie Joystick-/Orbit-Parkour-Spiel. Ziel: eigenständiges, gut lesbares Ein-Daumen-Spiel, besonders im Hochformat; Rising Steps bleibt das frei steuerbare Sprungspiel.

## Runde

Der Runner steht auf einem industriellen Aufzug oder Dachsteg über der Neonstadt. Das nächste bewegte Stockwerk und dessen Landebucht sind immer sichtbar. Eine breite Aktion unten löst den Wechsel aus. Der Spieler entscheidet über den Zeitpunkt, nicht über die Flugrichtung. Während des Wechsels keine weiteren Starts; keine Kameradrehung, kein Joystick.

Bewegte Zielplattformen kreuzen die vorgegebene Landeposition. Ein sichtbarer Zielbereich und eine kurze Flugbahn-Vorschau erklären das Timing. Landing wird anhand der tatsächlichen Deckposition zur tatsächlichen Ankunft ausgewertet; kein automatisches Nachführen zum bewegten Ziel. Zentrierte Landungen erzeugen Perfect-Ketten, Ton, kurze Haptik und Bonus. Knappes Timing ist ein normaler Treffer, außerhalb der Bucht ein Fehlversuch. Frühe Etagen haben breite Buchten und langsamere Bewegung.

30 Etagen pro Turm, mit sicheren Checkpoints alle fünf Etagen. Ein Fall bringt den Runner sofort zum letzten Checkpoint; Wiederholen startet niemals ungefragt bei null. Bereits belohnte Etagen liefern nach einem Fall keine zweite Belohnung. Am Ende wird die Runde abgeschlossen.

## Risiko und Abwechslung

Erfolgreiche neue Landungen sammeln eine sichtbare Rundenbelohnung getrennt vom vorhandenen Wallet. An Checkpoints: **Sichern** zahlt diese Belohnung genau einmal aus und beendet die Runde; **Eine Etage mehr** lässt sie für einen höheren Bonus im Risiko. Ein Fall verliert nur den ungesicherten Rundenteil, niemals gekaufte oder bereits gesicherte Coins. Checkpointfortschritt bleibt bestehen.

Drei Turmthemen verwenden unterschiedliche Aufzuggeschwindigkeiten, Richtungswechsel und bewegte Landebuchten. Bonus-Etagen bieten größere Perfect-Belohnungen; eine tägliche deterministische Route ergänzt die normalen Türme. Die ersten Etagen führen Timing, Perfects und Risiko einzeln ein. Cosmetics und langfristige Sterne/Bestleistungen bleiben; keine kostenpflichtige Wiederbelebung nötig.

## Ansicht und Bedienung

Feste dreiviertel-Seitenansicht, Translation folgt der Etage, Rotation bleibt konstant. Kamera-Rahmen umfasst immer Ausgangsdeck, Flugbahn und nächste Landebucht. Im Hochformat kompakter HUD oben, Spielfläche mittig, breite Aktion unten; im Querformat dieselbe eine Aktion im sicheren Bereich. Mindestens48 logische Punkte, Platz für Geräteausschnitte. Einblendungen nur in ruhenden Zuständen, kein Menü über dem aktiven Wechsel. DE/EN, Sound/Haptik, reduzierter Bewegungsmodus und hoher Kontrast bleiben.

Die bestehende Neonstadt, dunkle Industrial-Decks, goldene Routenkanten, cyanfarbene Aufzüge und der schwarze Hoodie-Runner bleiben erhalten und werden auf dieses Spielprinzip ausgerichtet. Rising Steps bleibt die helle schwebende Inselwelt.

## Umsetzung und Daten

Eigene deterministische Timing-/Risiko-Regeln, bewegte Decks, ein geführter Wechsel-Controller, feste Kamera und angepasstes HUD. Bestehende reale Store-/AdMob-Adapter, Produkt-IDs und dauerhafte Entitlements bleiben erhalten. Bestehende Profile behalten Wallet, Designs, Käufe, Sprache und Einstellungen. Neue Rundenfelder werden kompatibel ergänzt; ein altes laufendes Parkour-Spiel wird am gespeicherten Checkpoint in den neuen Ablauf überführt.

Pause, Hintergrund, Modals und OS-Kauf-/Einwilligungsdialoge frieren den Ablauf ein; Eingaben und gehaltene Pointer werden freigegeben. Rewards und Sichern sind idempotent, auch bei Reload. Keine Zahlung oder Werbung läuft automatisch.

## Nachweis vor interner Auslieferung

Regeltests: früh/zentriert/spät, bewegte Landebucht, Perfect-Kette, Checkpoint/Fall, Risiko/Sichern, Replay- und Reload-Schutz, Altprofil/Kaufbesitz. Tatsächlicher Spieler: vollständige30-Etagen-Runden, absichtliche Fehlschläge und Bonus-Etagen; Kamerawinkel bei Wechsel und Bewegung konstant. Hochformat und Querformat auf kompaktem iPhone sowie iPhone16ProMax, Screenshots mit freien Zielbuchten, erreichbaren Touch-Flächen und DE/EN. Native Builds und Signaturen; nur interne TestFlight-Verteilung. Physische Haptik/Temperatur und echte Sandbox-/Werbetests bleiben ausdrücklich separate Prüfungen.
