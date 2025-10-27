# Práctica 5: Escenas Cardboard

### Crear el proyecto Cardboard y experimentar y generar una apk para Android.
Se ha utilizado la escena por defecto de Cardboard para entender cómo funciona y generar una apk para Android. 
[APK generado](./CardboardApp.apk)

### Crea una escena con Cardboard que tenga objetos que al llegar el jugador a ellos los recolecte con la vista. El jugador se puede desplazar con un mando o con la vista. La escena debe contener un terreno y objetos de algún paquete de la Asset Store.

Se descargaron assets de manzanas y fresas para recolectar y un terreno desde la Asset Store de un bosque. Se ha implementado el sistema de recolección y movimiento con la vista.

- [`Assets/Scripts/GazeInteract.cs`](./Assets/Scripts/GazeInteract.cs)
  - Campos públicos: `collectGazeTime`, `moveGazeTime`, `moveSpeed`.
  - Permite recolectar objetos mirando durante `collectGazeTime` (envía al `Collector` si existe; si no, hace `RecallToTarget(this.transform)` como fallback).
  - Si no se mira un `Collectable`, mirar un punto en el mundo durante `moveGazeTime` hace que el jugador se mueva hacia ese punto.

- [`Assets/Scripts/Collectable.cs`](./Assets/Scripts/Collectable.cs)
  - Campos públicos: `isCollected`, `isStored` (internos para el estado).
  - Métodos:
    - `SendToCollector(Transform collector)` — marca `isCollected` y anima el objeto hacia el collector; queda como hijo y marcado como almacenado (`isStored = true`).
    - `RecallToTarget(Transform target)` — fuerza `isCollected = true`, anima el objeto hacia un punto frente a la cámara (altura de los ojos), lo rota mirando a la cámara y lo desactiva al final.
  - Parámetros principales a ajustar si se desea (ahora constantes o literales en el script): distancia delante de la cámara y duración de la animación.

### Elige un objeto en la escena que sirva para recuperar los que se recolectan. Cuando el jugador lo selecciona con la vista los recolectables se dirigen hacia el jugador.

Se ha elegido un objeto `Collector` (un cubo medio transparente) que al mirarlo hace que todos los objetos recolectados se dirijan hacia el jugador.

- [`Assets/Scripts/Collector.cs`](./Assets/Scripts/Collector.cs)
  - Campos públicos: `gazeTime`, `recallTarget`.
  - Comportamiento: cuando el jugador mira el objeto `Collector` durante `gazeTime`, el `Collector` busca todos los `Collectable` en la escena y llama `RecallToTarget` sólo a los que tienen `isCollected == true`.
  - Selección del objetivo (`target`) con la siguiente prioridad:
    1. `Camera.main.transform` (la cámara principal, que representa los ojos).
    2. Una cámara hija dentro del GameObject con tag `Player` (si existe).
    3. `recallTarget` (campo público si se asigna manualmente).
    4. `playerObj.transform` (el transform del GameObject con tag `Player`).


El apk generado con la escena se encuentra [aquí](./Bosque.apk).

### Vídeo de la práctica
[![Vídeo de la práctica](https://github.com/user-attachments/assets/04f60df9-4be3-435b-8340-7c89dd682764)](https://github.com/user-attachments/assets/04f60df9-4be3-435b-8340-7c89dd682764)