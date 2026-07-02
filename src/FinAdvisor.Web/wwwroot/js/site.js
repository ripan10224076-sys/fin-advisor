const savedTheme = localStorage.getItem("finadvisor-theme");
if (savedTheme === "dark") document.body.classList.add("dark");

document.getElementById("themeToggle")?.addEventListener("click", () => {
    document.body.classList.toggle("dark");
    localStorage.setItem("finadvisor-theme", document.body.classList.contains("dark") ? "dark" : "light");
});

document.querySelectorAll("form[data-loading]").forEach(form => {
    form.addEventListener("submit", () => {
        const button = form.querySelector("button[type='submit']");
        if (button) button.innerText = "Memproses...";
    });
});
