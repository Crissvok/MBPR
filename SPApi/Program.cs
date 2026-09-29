using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SPApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 

builder.Services.AddDbContext<AppDbContext>(Options => Options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();



app.MapGet("/", () => "Hello World!");
app.MapGet("/hello", () => "Привет вафля");
app.MapGet("/hello/{name}", (string name) =>
{
    return $"Привет, {name}!";
});
app.MapGet("/health", () => new
{
    status = "ok",
    service = "SPApi"
});
app.MapGet("/config", (IConfiguration config) => new
{
    name = config["Appinfo:Name"],
    version = config["Appinfo:Version"],
    author = config["Appinfo:Author"]
});
/*app.MapGet("/tetris", () => Results.Content(@"
<!DOCTYPE html>
<html lang='ru'>
<head>
    <meta charset='UTF-8'>
    <title>Мини-Тетрис с ускорением</title>
    <style>
        body { background: #111; color: #fff; font-family: sans-serif; text-align: center; margin-top: 30px; }
        .game-container { display: flex; justify-content: center; align-items: flex-start; gap: 20px; width: 440px; margin: 20px auto; }
        canvas { border: 2px solid #fff; background: #000; }
        .sidebar { display: flex; flex-direction: column; align-items: center; }
        #score-board { font-size: 20px; margin-bottom: 10px; line-height: 1.5; }
        #controls { color: #aaa; font-size: 14px; margin-bottom: 10px; }
        .next-title { font-size: 16px; margin-bottom: 5px; color: #0DC2FF; }
        .speed-highlight { color: #FF8E0D; font-weight: bold; }
    </style>
</head>
<body>
    <h1>Тетрис API</h1>
    <div id='score-board'>
        <div>Очки: <span id='score'>0</span></div>
        <div>Скорость: <span id='speed-lvl' class='speed-highlight'>1</span> (<span id='speed-ms'>1000</span> мс)</div>
    </div>
    <div id='controls'>Управление: Стр. Влево/Вправо, Вниз | Q/W - вращение | <b>R - Перезапуск</b></div>
    
    <div class='game-container'>
        <canvas id='tetris' width='240' height='400'></canvas>
        
        <div class='sidebar'>
            <div class='next-title'>Дальше:</div>
            <canvas id='next' width='100' height='260'></canvas>
        </div>
    </div>

    <script>
        const canvas = document.getElementById('tetris');
        const context = canvas.getContext('2d');
        context.scale(20, 20);

        const nextCanvas = document.getElementById('next');
        const nextContext = nextCanvas.getContext('2d');
        nextContext.scale(20, 20);

        function arenaSweep() {
            let rowsCleared = 0;
            outer: for (let y = arena.length - 1; y >= 0; --y) {
                for (let x = 0; x < arena[y].length; ++x) {
                    if (arena[y][x] === 0) { continue outer; }
                }
                arena.splice(y, 1);
                arena.unshift(new Array(12).fill(0)); 
                ++y;
                rowsCleared++;
            }
            
            if (rowsCleared === 1) player.score += 10;
            else if (rowsCleared === 2) player.score += 30;
            else if (rowsCleared === 3) player.score += 90;
            else if (rowsCleared === 4) player.score += 800;

            updateSpeedDynamic();
        }

        function updateSpeedDynamic() {
            const level = Math.floor(player.score / 100) + 1;
            dropInterval = Math.max(150, 1000 - (level - 1) * 50);
            
            document.getElementById('speed-lvl').innerText = level;
            document.getElementById('speed-ms').innerText = dropInterval;
        }

        function collide(arena, player) {
            const [m, o] = [player.matrix, player.pos];
            for (let y = 0; y < m.length; ++y) {
                for (let x = 0; x < m[y].length; ++x) {
                    if (m[y][x] !== 0 && (arena[y + o.y] && arena[y + o.y][x + o.x]) !== 0) {
                        return true;
                    }
                }
            }
            return false;
        }

        function createMatrix(w, h) {
            const matrix = [];
            while (h--) { matrix.push(new Array(w).fill(0)); }
            return matrix;
        }

        // Абсолютно надежный метод генерации матриц через разбор строк
        function parseMatrixString(str) {
            return str.trim().split('\n').map(row => row.trim().split('').map(Number));
        }

        function createPiece(type) {
            if (type === 'T') return parseMatrixString('010\n111\n000');
            if (type === 'O') return parseMatrixString('22\n22');
            if (type === 'L') return parseMatrixString('003\n333\n000');
            if (type === 'J') return parseMatrixString('400\n444\n000');
            if (type === 'I') return parseMatrixString('0000\n5555\n0000\n0000');
            if (type === 'S') return parseMatrixString('066\n660\n000');
            if (type === 'Z') return parseMatrixString('770\n077\n000');
        }

        function drawMatrix(matrix, offset, ctx = context) {
            matrix.forEach((row, y) => {
                row.forEach((value, x) => {
                    if (value !== 0) {
                        ctx.fillStyle = colors[value];
                        ctx.fillRect(x + offset.x, y + offset.y, 1, 1);
                    }
                });
            });
        }

        function draw() {
            context.fillStyle = '#000';
            context.fillRect(0, 0, canvas.width, canvas.height);
            drawMatrix(arena, {x: 0, y: 0});
            drawMatrix(player.matrix, player.pos);

            nextContext.fillStyle = '#000';
            nextContext.fillRect(0, 0, nextCanvas.width, nextCanvas.height);
            
            player.nextQueue.forEach((pieceType, index) => {
                const matrix = createPiece(pieceType);
                const offsetX = (5 - matrix.length) / 2;
                const offsetY = index * 4.2 + 0.5;
                drawMatrix(matrix, {x: offsetX, y: offsetY}, nextContext);
            });
        }

        function merge(arena, player) {
            player.matrix.forEach((row, y) => {
                row.forEach((value, x) => {
                    if (value !== 0) { arena[y + player.pos.y][x + player.pos.x] = value; }
                });
            });
        }

        function rotate(matrix, dir) {
            for (let y = 0; y < matrix.length; ++y) {
                for (let x = 0; x < y; ++x) {
                    [matrix[x][y], matrix[y][x]] = [matrix[y][x], matrix[x][y]];
                }
            }
            if (dir > 0) { matrix.forEach(row => row.reverse()); } 
            else { matrix.reverse(); }
        }

        function playerDrop() {
            player.pos.y++;
            if (collide(arena, player)) {
                player.pos.y--;
                merge(arena, player);
                playerReset();
                arenaSweep();
                updateStats();
            }
            dropCounter = 0;
        }

        function playerMove(offset) {
            player.pos.x += offset;
            if (collide(arena, player)) { player.pos.x -= offset; }
        }
        function playerHardDrop() {
            while (!collide(arena, player)) {
                player.pos.y++;
            }
            player.pos.y--; // Возвращаем в последнюю безопасную позицию
            merge(arena, player);
            playerReset();
            arenaSweep();
            updateScore();
            dropCounter = 0;
        }

        function resetGame() {
            arena.forEach(row => row.fill(0));
            player.score = 0;
            player.nextQueue = [];
            for(let i = 0; i < 4; i++) { fillNextQueue(); } 
            playerReset();
            updateStats();
        }

        function fillNextQueue() {
            const pieces = 'TJLOSZI';
            player.nextQueue.push(pieces[pieces.length * Math.random() | 0]);
        }

        function playerReset() {
            const currentPieceType = player.nextQueue.shift();
            fillNextQueue();

            player.matrix = createPiece(currentPieceType);
            player.pos.y = 0;
            player.pos.x = (arena.length / 2 | 0) - (player.matrix.length / 2 | 0);
            
            if (collide(arena, player)) {
                resetGame();
            }
        }

        function playerRotate(dir) {
            const pos = player.pos.x;
            let offset = 1;
            rotate(player.matrix, dir);
            while (collide(arena, player)) {
                player.pos.x += offset;
                offset = -(offset + (offset > 0 ? 1 : -1));
                if (offset > player.matrix.length) {
                    rotate(player.matrix, -dir);
                    player.pos.x = pos;
                    return;
                }
            }
        }

        let dropCounter = 0;
        let dropInterval = 1000;
        let lastTime = 0;

        function update(time = 0) {
            const deltaTime = time - lastTime;
            lastTime = time;
            dropCounter += deltaTime;
            if (dropCounter > dropInterval) { playerDrop(); }
            draw();
            requestAnimationFrame(update);
        }

        function updateStats() {
            document.getElementById('score').innerText = player.score;
            updateSpeedDynamic();
        }

        const colors = [null, '#FF0D72', '#0DC2FF', '#0DFF72', '#F538FF', '#FF8E0D', '#FFE138', '#3877FF'];
        const arena = createMatrix(12, 20);
        const player = { pos: {x: 0, y: 0}, matrix: null, score: 0, nextQueue: [] };

        document.addEventListener('keydown', event => {
            if (event.code === 'KeyR') { resetGame(); }
            else if (event.code === 'Space') { 
                event.preventDefault(); // Запрещает прокрутку страницы вниз при нажатии
                playerHardDrop(); 
            }
            else if (event.keyCode === 37) { playerMove(-1); }
            else if (event.keyCode === 39) { playerMove(1); }
            else if (event.keyCode === 40) { playerDrop(); }
            else if (event.keyCode === 81) { playerRotate(-1); }
            else if (event.keyCode === 87) { playerRotate(1); }
        });

        for(let i = 0; i < 4; i++) { fillNextQueue(); }
        playerReset();
        updateStats();
        update();
    </script>
</body>
</html>
", "text/html; charset=utf-8"));
*/

app.MapControllers();

app.Run();


















/*

*/


