# Software Dependency Analyzer for NPM Projects

Web aplikacija za analizu softverskih zavisnosti NPM projekata. Omogućava pregled direktnih i tranzitivnih zavisnosti, njihovih međusobnih odnosa, kao i analizu potencijalnih bezbednosnih rizika koji mogu uticati na projekat.

## Koji problem aplikacija rešava?

U savremenom razvoju softvera, NPM projekti često koriste veliki broj biblioteka, koje same mogu zavisiti od drugih biblioteka. Zbog toga je teško imati jasan pregled kompletne strukture zavisnosti i proceniti kako problem u jednoj biblioteci može uticati na ostatak projekta.

Aplikacija rešava ovaj problem analizom `package-lock.json` fajla, na osnovu kojeg identifikuje sve zavisnosti projekta i njihove međusobne veze.

Kroz intuitivan korisnički interfejs i interaktivni graf, korisniku se omogućava pregled strukture projekta, jednostavno pronalaženje konkretnih paketa i identifikacija potencijalnih bezbednosnih i drugih problema.

## Funkcionalnosti

Aplikacija implementira sledeće funkcionalnosti:

- **Analiza zavisnosti** – pronalaženje direktnih i tranzitivnih zavisnosti projekta.
- **Dubina zavisnosti** – određivanje nivoa na kojem se određeni paket nalazi u hijerarhiji zavisnosti.
- **Analiza uticaja ranjivosti** – identifikacija paketa koji mogu biti pogođeni bezbednosnom ranjivošću i analiza njenog uticaja na projekat.
- **Detekcija duplikata** – pronalaženje različitih verzija istog paketa unutar projekta.
- **Najkraći put do paketa** – pronalaženje najkraćeg puta kroz graf zavisnosti od korenskog projekta do odabranog paketa.
- **Detekcija cikličnih zavisnosti** – identifikacija ciklusa u grafu zavisnosti.
- **Provera kompatibilnosti licenci** – analiza licenci korišćenih biblioteka i potencijalnih problema sa njihovom kompatibilnošću.
- **Interaktivni graf zavisnosti** – vizuelni prikaz strukture projekta sa mogućnošću interakcije i istraživanja pojedinačnih paketa.

## Tehnologije

## Arhitektura aplikacije

## Pokretanje projekta

## Primeri korišćenja

## Screenshots