let currentPlayer = "X";
let gameMode = "multiplayer"; 

// Set up SignalR connection
const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

// Start the connection
connection.start().catch(err => console.error("SignalR Connection Error:", err));

// Add event listeners to game mode buttons
document.getElementById("multiplayerBtn").addEventListener("click", () => {
    gameMode = "multiplayer";
    resetGame();
    document.getElementById("multiplayerBtn").classList.add("active");
    document.getElementById("singlePlayerBtn").classList.remove("active");
    console.log("Game mode set to multiplayer");
});

document.getElementById("singlePlayerBtn").addEventListener("click", () => {
    gameMode = "single";
    resetGame();
    document.getElementById("singlePlayerBtn").classList.add("active");
    document.getElementById("multiplayerBtn").classList.remove("active");
    console.log("Game mode set to single player");
});

// Add click event listeners to all cells
document.querySelectorAll(".cell").forEach(cell => {
    cell.addEventListener("click", function () {
        const index = parseInt(this.dataset.index);

        if (this.innerText.trim()) return;

        if (gameMode === "multiplayer") {
            connection.invoke("MakeMove", index, currentPlayer)
                .catch(err => console.error("Error invoking MakeMove:", err));
        } else {
            singlePlayerMove(index);
        }
    });
});

// Handle single player moves
function singlePlayerMove(index) {
    fetch("/Game/MakeMove", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ index: index, player: "X" })
    })
        .then(response => response.json())
        .then(data => {
            console.log("Response from server:", data);

            updateBoard(index, "X");

            if (data.status === "win") {
                setTimeout(() => {
                    alert(`${data.winner} wins!`);
                    updateStats();
                }, 100);
                return;
            }

            if (data.status === "next" && data.aiMove !== -1) {
                setTimeout(() => {
                    updateBoard(data.aiMove, "O");

                    fetch("/Game/GetStats")
                        .then(response => response.json())
                        .then(stats => {
                            if (stats.oWins > 0) {
                                setTimeout(() => {
                                    alert("O wins!");
                                    updateStats();
                                }, 100);
                            }
                        });

                }, 500);
            }
        })
        .catch(err => console.error("Error making move:", err));
}



// Update stats display from server
function updateStats() {
    fetch("/Game/GetStats")
        .then(response => response.json())
        .then(data => {
            document.getElementById("xWins").textContent = data.xWins;
            document.getElementById("oWins").textContent = data.oWins;
            document.getElementById("draws").textContent = data.draws;
        })
        .catch(err => console.error("Error fetching stats:", err));
}

// Handle SignalR events for multiplayer
connection.on("ReceiveMove", (index, player) => {
    updateBoard(index, player);
    currentPlayer = (player === "X") ? "O" : "X";
});

connection.on("GameOver", (winner) => {
    if (winner === "Draw") {
        setTimeout(() => alert("It's a draw!"), 100);
    } else {
        setTimeout(() => alert(`${winner} wins!`), 100);
    }
    updateStats();
});

connection.on("ReceiveStats", (xWins, oWins, draws) => {
    document.getElementById("xWins").textContent = xWins;
    document.getElementById("oWins").textContent = oWins;
    document.getElementById("draws").textContent = draws;
});

// Reset button event listener
document.getElementById("resetBtn").addEventListener("click", resetGame);

function resetGame() {
    if (gameMode === "multiplayer") {
        connection.invoke("ResetGame").catch(err => console.error("Error resetting game:", err));
    } else {
        fetch("/Game/ResetGame", {
            method: "POST"
        })
            .then(response => response.json())
            .then(() => {
                clearBoard();
                currentPlayer = "X";
            })
            .catch(err => console.error("Error resetting game:", err));
    }
}

// Handle the SignalR reset event
connection.on("ResetBoard", () => {
    clearBoard();
    currentPlayer = "X";
});

function updateBoard(index, player) {
    const cell = document.querySelector(`.cell[data-index='${index}']`);
    if (cell) {
        cell.innerText = player;
        cell.style.color = player === "X" ? "red" : "blue";
    }
}

// Clear the board for a new game
function clearBoard() {
    document.querySelectorAll(".cell").forEach(cell => {
        cell.innerText = "";
    });
}

// Load stats when page loads
document.addEventListener("DOMContentLoaded", () => {
    updateStats();
});