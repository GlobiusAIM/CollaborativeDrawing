// Функция для получения координат canvas
window.getCanvasRect = (canvas) => {
    if (!canvas) return { left: 0, top: 0, width: 0, height: 0 };
    
    const rect = canvas.getBoundingClientRect();
    return {
        left: rect.left,
        top: rect.top,
        width: rect.width,
        height: rect.height
    };
};

// Функция для рисования точки
window.drawPoint = (canvas, point) => {
    if (!canvas || !point) return;
    
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    
    ctx.lineWidth = point.lineWidth;
    ctx.strokeStyle = point.color;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';

    if (point.isStartOfStroke) {
        ctx.beginPath();
        ctx.moveTo(point.x, point.y);
    } else {
        ctx.lineTo(point.x, point.y);
        ctx.stroke();
    }
};

// Функция для очистки canvas
window.clearCanvas = (canvas) => {
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    // Заполняем белым фоном
    ctx.fillStyle = '#ffffff';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
};

// Инициализация canvas с правильными размерами
window.initializeCanvas = (canvas) => {
    if (!canvas) return;
    
    const resizeCanvas = () => {
        const container = canvas.parentElement;
        if (!container) return;
        
        // Сохраняем текущее содержимое
        const ctx = canvas.getContext('2d');
        const imageData = ctx ? ctx.getImageData(0, 0, canvas.width, canvas.height) : null;
        
        // Устанавливаем новые размеры
        canvas.width = container.clientWidth;
        canvas.height = container.clientHeight;
        
        // Восстанавливаем содержимое или создаем белый фон
        if (ctx) {
            if (imageData && imageData.data.some(value => value !== 0)) {
                ctx.putImageData(imageData, 0, 0);
            } else {
                ctx.fillStyle = '#ffffff';
                ctx.fillRect(0, 0, canvas.width, canvas.height);
            }
        }
    };

    // Вызываем сразу и при изменении размера окна
    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);
    
    return {
        width: canvas.width,
        height: canvas.height
    };
};