// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    const searchBox = document.getElementById('searchBox');
    const suggestions = document.getElementById('searchSuggestions');
    if (!searchBox || !suggestions) return;

    searchBox.addEventListener('input', function () {
        const query = this.value.trim();
        if (query.length < 2) {
            suggestions.style.display = 'none';
            return;
        }
        fetch(`/Home/SearchProduct?term=${encodeURIComponent(query)}`)
            .then(res => res.json())
            .then(data => {
                if (!data || data.length === 0) {
                    suggestions.style.display = 'none';
                    return;
                }
                suggestions.innerHTML = data.map(item => `
                    <div class="search-suggestion-item" onclick="window.location='/Home/Details/${item.id}'">
                        <img src="${item.imageUrl}" alt="${item.name}" />
                        <div>
                            <div>${item.name}</div>
                            <div style='color:red;'>${item.price.toLocaleString()}₫</div>
                        </div>
                    </div>
                `).join('');
                suggestions.style.display = 'block';
            });
    });

    document.addEventListener('click', function (e) {
        if (!searchBox.contains(e.target) && !suggestions.contains(e.target)) {
            suggestions.style.display = 'none';
        }
    });
});
