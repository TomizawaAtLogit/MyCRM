// Dashboard Chart Helper
window.DashboardCharts = {
    createBarChart: function (canvasId, labels, data, title) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        if (window.chartInstances && window.chartInstances[canvasId]) {
            window.chartInstances[canvasId].destroy();
        }

        const chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: title,
                    data: data,
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.6)',
                        'rgba(75, 192, 192, 0.6)',
                        'rgba(255, 206, 86, 0.6)',
                        'rgba(153, 102, 255, 0.6)',
                        'rgba(255, 159, 64, 0.6)'
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    title: {
                        display: true,
                        text: title
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });

        if (!window.chartInstances) {
            window.chartInstances = {};
        }
        window.chartInstances[canvasId] = chart;
        return chart;
    },

    createPieChart: function (canvasId, labels, data, title) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        if (window.chartInstances && window.chartInstances[canvasId]) {
            window.chartInstances[canvasId].destroy();
        }

        const chart = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.6)',
                        'rgba(255, 159, 64, 0.6)',
                        'rgba(255, 205, 86, 0.6)',
                        'rgba(75, 192, 192, 0.6)',
                        'rgba(54, 162, 235, 0.6)',
                        'rgba(153, 102, 255, 0.6)',
                        'rgba(201, 203, 207, 0.6)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(255, 159, 64, 1)',
                        'rgba(255, 205, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(201, 203, 207, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right',
                    },
                    title: {
                        display: true,
                        text: title
                    }
                }
            }
        });

        if (!window.chartInstances) {
            window.chartInstances = {};
        }
        window.chartInstances[canvasId] = chart;
        return chart;
    },

    createLineChart: function (canvasId, labels, datasets, title) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        if (window.chartInstances && window.chartInstances[canvasId]) {
            window.chartInstances[canvasId].destroy();
        }

        const chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: datasets
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: title
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });

        if (!window.chartInstances) {
            window.chartInstances = {};
        }
        window.chartInstances[canvasId] = chart;
        return chart;
    },

    destroyChart: function (canvasId) {
        if (window.chartInstances && window.chartInstances[canvasId]) {
            window.chartInstances[canvasId].destroy();
            delete window.chartInstances[canvasId];
        }
    }
};
