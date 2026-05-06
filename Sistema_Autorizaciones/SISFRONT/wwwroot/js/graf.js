window.generarGraficoGiros = (labels, datasets) => {
    const ctx = document.getElementById('graficoGiros').getContext('2d');

    if (window.miGraficoGiros) {
        window.miGraficoGiros.destroy(); // Evita duplicados
    }

    window.miGraficoGiros = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: datasets
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        font: { size: 14 }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.dataset.label}: ${context.parsed.y}`;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1,
                        precision: 0
                    }
                }
            }
        }
    });
};
