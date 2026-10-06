# Ball Obstacle

Actividad en Unity 6000.5.9f1 hecha con esferas, cubos y cilindros.

Abre `Assets/Scenes/BallObstacle.unity` y presiona Play.

- Mueve la pelota con las flechas. El movimiento usa fuerza directa, sin torque.
- Recoge los tres cubos amarillos y llega al bloque verde para ver "You win!".
- Hay dos rampas, un camino estrecho y cuatro pilares que puedes derribar.
- La plataforma cambia de inclinacion cada cuatro segundos.
- Si caes, vuelves al inicio y conservas los objetos recogidos.
- El boton Reiniciar recarga la escena: restaura la pelota, los objetos, el contador, los pilares y la plataforma.

La interfaz usa los controles normales de Canvas de Unity, sin estilos personalizados.
`SampleScene` conserva la escena inicial. La escena del juego esta incluida en Build Profiles.

Scripts del juego: `BallController`, `PlatformTilt` y `GameManager`.
El menu `Activity > Create Ball Obstacle` permite volver a generar el nivel inicial.
