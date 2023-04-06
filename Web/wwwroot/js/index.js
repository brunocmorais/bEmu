function gameLoop() {
    window.requestAnimationFrame(gameLoop);
    game.instance.invokeMethodAsync('GameLoop');
}

window.initGame = function (instance) {
    var canvas = document.getElementsByTagName("canvas")[0];

    window.game = {
        instance: instance,
        canvas: canvas
    };
    window.requestAnimationFrame(gameLoop);
};

window.writeFramebuffer = function (framebuffer, width, height) {

    var canvas = document.getElementsByTagName("canvas")[0];
    var context = canvas.getContext("2d");

    canvas.width = width;
    canvas.height = height;

    var array = new Uint8ClampedArray(framebuffer);
    var imageData = new ImageData(array, width, height);

    context.putImageData(imageData, 0, 0, 0, 0, canvas.width, canvas.height);
}