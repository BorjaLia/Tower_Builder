# Corrección — TP02 Tower Builder

---

## 1. Resumen general

El proyecto es un *Tower Builder* sólido y bien estructurado a nivel de arquitectura: usa Singletons
persistentes, un sistema de eventos (Observer) para desacoplar la UI, ScriptableObjects para la
configuración de bloques y autos, AudioMixer con sliders, persistencia con PlayerPrefs, menú de pausa,
modelos 3D reales (Kenney/KayKit) con materiales, SFX y Git LFS. El nivel técnico es claramente de Tier S.

Sin embargo, la rúbrica usa un **sistema de compuerta**: los tiers superiores solo puntúan si los
inferiores están **completos**. Y hay **un requisito del Tier B incompleto**: el HUD de gameplay **no
muestra la altura de la torre** (B3 lo exige explícitamente, junto con puntaje y racha). El dato de altura
existe internamente (`currentHeight` en `GameplayManager`) pero solo se usa para mover la cámara, nunca se
muestra en pantalla, y no hay ningún campo de texto de altura ni en `GameplayHUD.cs` ni en la escena
`GameplayScene.unity`.

Por la regla de compuerta, ese único faltante en Tier B impide sumar los puntos (excelentes) de Tier A y
Tier S. Es un caso claro de "muy buen trabajo, frenado por un detalle de la consigna".

---

## 2. Checklist por tier

### TIER B (base, req. 1–4)

| Req | Estado      | Justificación|
|-----|-------------|--------------|
| **B1** Mecánica y Físicas | ✓           | `BlockController` usa `Rigidbody` + `BoxCollider` (`[RequireComponent]`), arranca kinematic pegado a la grúa y al soltar (`DropBlock`) cae y se apila. El generador (`BlockSpawner.MoveSpawner`) se mueve horizontal de forma automática y continua. Movimiento oscilante constante: cumple. Único detalle, se pierde por acumulación y no por errarle al ultimo bloque puesto |
| **B2** Flujo de Escenas | ✓           | Dos escenas: `MainMenuScene.unity` y `GameplayScene.unity`. Transición vía `MainMenuManager.PlayGame()` → `SceneManager.LoadScene("GameplayScene")` y `GameplayManager.ReturnToMenu()`.|
| **B3** UI Básica | ✗           | Menú principal con los 4 botones funcionales (en escena: `Play`, `Settings`, `Credits`, `Exit`. HUD muestra **puntaje** (`scoreText`) y **racha/strikes** (`strikesText`, con "Perfect!"/"STREAK x{n}!"). **FALTA la altura de la torre**: no hay texto de altura en `GameplayHUD.cs` ni en la escena. Por eso no es ✓.|
| **B4** Manejo de assets | **Parcial** | Modelos `.fbx` reales para bloques (`Art/Models/blocks/building.fbx`, `building-b.fbx`), autos y edificios. Materiales aplicados (`HeavyMaterial.mat`, `LightMaterial.mat`). SFX al colisionar (`BlockHit`, `BlockHitPerfect`); Git LFS presente (`.gitattributes` con png/fbx/mat/tga/wav/ogg). Hay primitivas para la UI, botones y sliders son placeholders.|

**Tier B: 3 de 4 completos (B3 Parcial) → Tier B NO está completo. La compuerta hacia Tier A se cierra.**

### TIER A (req. 5–7) — *los puntos solo cuentan si TODO Tier B está completo*

| Req | Estado | Justificación                                                                                                                                                                         |
|-----|-------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **A1** Menú de Pausa con tecla | ✓ | `PauseMenuManager.Update()` escucha `KeyCode.Escape` → `GameplayManager.TogglePause()` (setea `Time.timeScale` y dispara `OnPauseToggled`). Maneja además cerrar settings con Escape. |
| **A2** Configuración y Mixers | ✓ | `SettingsManager` con 4 sliders (Master, Music, SFX, UI).                                                                                                                             |
| **A3** Persistencia | ✓ | `DataManager` (PlayerPrefs): guarda/recupera highscore (`SaveHighScore`, `HIGH_SCORE_KEY`) y los 4 volúmenes (`SaveVolumes` / `LoadData`).                                            |

**Tier A está técnicamente bien resuelto, pero por la compuerta de B3 NO suma puntos.**

### TIER S (req. 8–10) — *solo cuenta si TODO Tier A cuenta (y por ende todo Tier B)*

| Req | Estado (cumplido en sí mismo) | Justificación|
|-----|--------------------------|------|
| **S1** Singleton y Managers | ✓ | `Singleton<T>` y `PersistentSingleton<T>` (con `DontDestroyOnLoad`). `AudioManager` y `DataManager` son persistentes y `GameplayManager` es singleton de escena. Persisten entre escenas.|
| **S2** Observer y ScriptableObjects | ✓ | Observer: eventos `Action` en `GameplayManager` (`OnScoreUpdated`, `OnStrikeUpdated`, `OnTowerPosUpdated`, `OnLivesUpdated`, `OnGameOver`, `OnPauseToggled`, `OnTowerStabilized`); la UI (`GameplayHUD`) se actualiza desacoplada por suscripción. ScriptableObjects: `BlockData.cs` y `CarData.cs` con assets reales (`HeavyBlockData`, `LightBlockData`, 4 CarData) definiendo masa, damping, escala, colores/modelo, velocidad. |
| **S3** GitHub / Build / Itch.io | ✗ | tags `v1.0` y `v1.1` ✓, 14 commits ✓, `README.md` presente con link a Itch (https://borjaliad.itch.io/tower-builder) y autor. **PERO**: el README es muy breve y **no incluye "Cómo jugar"**; el "ocultar botón Exit en WebGL" **NO está implementado**. Release con build ejecutable. **No hay** ≥3 imágenes y video 20–40s en Itch.|

**Tier S está técnicamente muy bien (S1 y S2 ejemplares), pero por la compuerta NO suma puntos; además S3 tiene faltantes reales.**

---

## 3. Qué tiene / Qué le falta

### Qué tiene (fortalezas)
- Físicas correctas (Rigidbody/Collider, kinematic→dinámico al soltar, apilado real).
- Generador automático con dificultad dinámica según highscore.
- Dos escenas con flujo Menú ↔ Gameplay.
- Menú principal completo (Play/Settings/Credits/Exit funcionales).
- Modelos 3D reales + materiales + SFX + Git LFS, sin primitivas en los bloques.
- Menú de pausa con Escape.
- AudioMixer con 4 sliders accesible desde menú y pausa.
- Persistencia (highscore + volúmenes) con PlayerPrefs.
- Singletons persistentes, Observer por eventos y ScriptableObjects bien usados.
- Sistema extra de vidas, racha y cámara con deadzone (valor agregado).

### Qué le falta
- Todos los botones de UI son primitivas.
- **(Gate) Mostrar la altura de la torre en el HUD** — único faltante que cierra la compuerta de Tier B.
- Ocultar/deshabilitar el botón **Exit en WebGL** (requisito explícito de S3).
- README más completo: sección **"Cómo jugar"** y linkeo circular README↔Itch claro.
- Itch con ≥3 imágenes y video 20–40s.

---

## 4. Hallazgos de código más importantes

Se insertaron **comentarios en línea** (`// Error:` / `// Warning:` / `// Sugestion:`) en los scripts.
Los más relevantes:

- **Error — `GameplayHUD.cs`**: el HUD no tiene campo ni texto para la **altura** de la torre (B3).
- **Error — `MainMenuManager.cs`**: `ExitGame()` no oculta el botón ni protege con `#if UNITY_WEBGL`; `Application.Quit()` no tiene efecto en WebGL (S3).
- **Warning — `SceneryController.cs`**: campos `public` sin encapsular en `Car`, lista de instancias runtime con `[SerializeField]`, `AddComponent` de Rigidbody/Collider por código en cada auto, y manipulación de `transform.position` de objetos con Rigidbody dentro de `Update` (debería ir en `FixedUpdate`/`MovePosition`).
- **Warning — `BlockController.OnCollisionEnter`**: reproduce SFX en cada colisión (puede generar spam de audio).
- **Warning — `GameplayManager.cs`**: eventos `static` en un manager de escena (riesgo de subscriptores colgados entre reinicios; acá se desuscriben bien, pero es frágil).
- **Warning — `GameRigController.cs`**: `using static UnityEngine.GraphicsBuffer` sin usar; `smoothSpeed` es `Vector2` pero solo se usa `.x`.
- **Sugestion** varias: números mágicos (umbrales de dificultad, tolerancias, 0.1f de estabilidad, conversión dB), `print()` de debug dejados en la entrega (`BlockSpawner`, `DataManager`), propiedades en camelCase que deberían ser PascalCase, prefabs que deberían traer ya sus componentes, y `PersistentSingleton` que duplica código de `Singleton`.

---

## 5. Cálculo de la nota (regla de compuerta, paso a paso)

**Conteo base por requisito (1 punto cada uno):**

- **Tier B (1–4):** B1 ✓, B2 ✓, **B3 Parcial (no cuenta como cumplido)**, B4 ✓ → **3 puntos**.
- **Tier A (5–7):** A1 ✓, A2 ✓, A3 ✓ → 3 puntos *en bruto*.
- **Tier S (8–10):** S1 ✓, S2 ✓, S3 Parcial → 2 puntos *en bruto*.

**Aplicación de la compuerta:**

1. **Tier B**: para que cuenten los puntos de tiers superiores, **TODO** Tier B debe estar cumplido.
   B3 está **Parcial** (falta mostrar la altura) → **Tier B NO está completo**.
2. **Compuerta B→A**: como Tier B no está completo, los requisitos de **Tier A NO cuentan** (aunque estén
   bien implementados). → 0 puntos de Tier A.
3. **Compuerta A→S**: como Tier A no cuenta, los requisitos de **Tier S NO cuentan**. → 0 puntos de Tier S.

**Puntos válidos = solo los requisitos cumplidos de Tier B = B1 + B2 + B4 = 3.**

### **NOTA: 3/10**

> Nota: El trabajo *merecería* alrededor de 9 (+-) si B3 estuviera completo (Tier B=4, Tier A=3, Tier S=2 con S3 parcial). La compuerta es estricta y deja la nota en del trabajo en 3. 

> No es necesaria una re-entrega. Se sube la nota por **participación en clase** y **calidad de entrega**, omitiendo el faltante. Se entiende que el Lia sabe implementar lo restante y fue una omisión de lectura.

### **NOTA FINAL: 7/10**
