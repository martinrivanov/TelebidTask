export const generateCaptchaText = () => {
    var text = '';
    const symbols = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
    
    for (let i = 0; i < 6; i++) {
        text += symbols[Math.floor(Math.random() * symbols.length)]
    }

    return text;
}

export const drawCaptcha = (ctx, text) => {
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
    const letterSpace = (ctx.canvas.width - 25) / text.length;
    const horizontalSpace = 25;

    for (let i = 0; i < text.length; i++) {
        ctx.font = 'italic 20px Roboto Mono';
        ctx.fillStyle = 'rgb(0, 0, 0)';
        ctx.fillText(text[i], horizontalSpace + i * letterSpace, Math.floor(Math.random() * 16 + 25), 100);
    }
}