# 🎮 Simulazione Videoludica della Visual Snow Syndrome

## 📌 Descrizione del progetto

Questo progetto realizza una simulazione interattiva di disturbi percettivi visivi e sensoriali, tra cui:

* Neve Visiva
* Fotofobia
* Palinopsia
* Acufene
* Vignettatura periferica

L’obiettivo è riprodurre tali fenomeni attraverso tecniche di rendering in Unity (URP), Shader Graph, Scriptable Render Feature e gestione dell’interazione utente.

---

## 🧱 Struttura del progetto

### 📁 Assets

Contiene tutti gli asset principali del progetto.

---

### 📁 Effects

Gli effetti sono organizzati per **sintomo simulato**.
Ogni cartella contiene tutti gli asset necessari (shader, materiali, script, audio).

* **Acufene** → audio spazializzato e animazioni
* **Vignettatura** → shader e materiale
* **Fotofobia** → shader, materiale, script
* **Palinopsia** → shader, materiale, render feature
* **NeveVisiva** → volume profile e texture

---

### 📁 Input

Gestione degli input del giocatore:

* `InputSystem_Action` → configurazione Input System
* `PlayerInputHandler` → gestione logica input

---

### 📁 Scripts

Script principali del sistema:

* `FirstPersonController.cs` → movimento e visuale
* `PlayerInputHandler.cs` → gestione input
* `GameManagerBehavior.cs` → pausa e stato globale
* `CambioScena.cs` → gestione scene

---

### 📁 Models / Prefabs

* Modelli 3D importati da Blender
* Prefab pronti all’uso nelle scene

---

### 📁 Scenes

Scene del progetto:

* Camera da letto
* Corridoio

---

### 📁 Settings

Configurazioni della pipeline di rendering (URP), renderer e post-processing.

---

### 📁 Textures

Texture e immagini utilizzate negli effetti.

---

### 📁 Packages

Dipendenze del progetto Unity.

---

## ⚙️ Requisiti

* Unity (2022 o successivo, con URP)
* Input System attivo

---

## ▶️ Avvio del progetto

1. Aprire Unity Hub
2. Caricare il progetto
3. Aprire una scena
4. Premere Play

---

## 🎮 Controlli

* **WASD** → movimento
* **Mouse** → visuale
* **Shift** → sprint
* **Spazio** → salto
* **ESC** → pausa

---

## 🧠 Architettura del Sistema

Il sistema è organizzato in moduli indipendenti per separare input, logica e rendering.

```
[Input System]
        ↓
[PlayerInputHandler]
        ↓
[FirstPersonController]
        ↓
[Effetti visivi / Rendering]
        ↓
[Output a schermo]
```

---

### 🔷 Moduli principali

#### 🎮 Input Layer

Gestisce gli input tramite Input System e li rende disponibili agli altri sistemi.

#### 🧍 Player Controller

Gestisce movimento, rotazione e interazione nello spazio 3D.

#### ⚙️ Game Manager

Controlla stato globale, pausa e interfaccia utente.

#### 🎨 Effects & Rendering

Implementa la simulazione dei disturbi tramite:

* Shader Graph
* Script C#
* Post-processing
* Scriptable Render Feature

---

### 🔬 Implementazione degli effetti

* **Fotofobia**
  Calcolo dinamico dello stress visivo tramite raycast e aggiornamento shader

* **Palinopsia**
  Un blend di frame collegati temporalmente tra di loro usando i parametri della image feature della render pipeline

* **Neve Visiva**
  Applicazione di rumore tramite post-processing

* **Acufene**
  Riproduzione audio spazializzata persistente

* **Vignettatura**
  Oscuramento periferico tramite shader

---

### 🎯 Caratteristiche architetturali

* Modularità degli effetti
* Separazione delle responsabilità
* Facilità di estensione
* Integrazione con URP

---

## 🚀 Sviluppi Futuri

Il progetto può essere esteso in diverse direzioni, sia dal punto di vista tecnico che applicativo.

### 🥽 Integrazione con realtà virtuale (VR)

Un’evoluzione naturale del sistema consiste nell’adattamento alla realtà virtuale, al fine di aumentare il livello di immersione.

Possibili sviluppi:

* Integrazione con dispositivi VR (es. visori standalone o PC-based)
* Adattamento degli effetti visivi alla visione stereoscopica
* Ottimizzazione delle performance per mantenere frame rate elevati (requisito critico in VR)

Questo permetterebbe una simulazione più realistica dei disturbi percettivi, migliorando l’efficacia dell’esperienza.

---

### 🎨 Miglioramento della qualità grafica

Ulteriori sviluppi possono riguardare il miglioramento visivo complessivo del progetto:

* Ottimizzazione e raffinamento degli shader
* Miglioramento dell’illuminazione (Global Illumination, luci dinamiche)
* Maggiore dettaglio nei modelli 3D e nelle texture
* Introduzione di effetti visivi più avanzati e parametrizzabili

L’obiettivo è rendere la simulazione più credibile e visivamente coerente.

---

### 📱 Porting su dispositivi mobile

Il progetto può essere adattato per l’esecuzione su dispositivi mobili.

Sfide principali:

* Riduzione del costo computazionale degli shader
* Ottimizzazione del rendering per hardware limitato
* Adattamento dell’interfaccia utente a input touch

Possibili soluzioni:

* Utilizzo di pipeline di rendering semplificate
* Riduzione della complessità degli effetti visivi
* Profilazione delle performance su dispositivi reali

---

### 🔬 Estensioni funzionali

Ulteriori sviluppi possono includere:

* Parametrizzazione degli effetti (intensità, velocità, durata)
* Possibile utilizzo in contesti educativi o dimostrativi

---

## 🎯 Considerazioni finali

Le possibili estensioni evidenziano come il progetto possa evolvere da prototipo dimostrativo a piattaforma più completa per la simulazione di fenomeni percettivi, mantenendo un’architettura modulare e facilmente espandibile.


---
## 📌 Note

* Gli effetti dipendono da shader personalizzati
* Il progetto è sviluppato per finalità accademiche
* Le prestazioni possono variare in base all’hardware
