function gameLoop() {
    
    window.requestAnimationFrame(gameLoop);
    game.instance.invokeMethodAsync('GameLoop');
}

function getCanvas() { 
    return document.getElementsByTagName("canvas")[0]; 
}

window.initGame = function (instance) {
    
    window.game = {
        instance: instance,
        canvas: getCanvas()
    };

    window.requestAnimationFrame(gameLoop);

    if (document.activeElement != canvas)
        canvas.focus();
};

window.writeFramebuffer = function (framebuffer, width, height) {

    const canvas = getCanvas();
    const context = canvas.getContext("2d");

    canvas.width = width;
    canvas.height = height;

    const imageData = new ImageData(new Uint8ClampedArray(framebuffer), width, height);

    context.putImageData(imageData, 0, 0);
}