const studentsUrl = "/students/all";
const groupsUrl = "/groups/all";

async function loadGroups() {
  const response = await fetch(groupsUrl);
  if (!response.ok) throw new Error("Не удалось загрузить группы");
  const groups = await response.json();

  document.querySelector("#groupId").innerHTML =
    '<option value="">Без группы</option>' +
    groups.map(group =>
      `<option value="${group.id}">${escapeHtml(group.name)}</option>`
    ).join("");

  document.querySelector("#groups").innerHTML = groups.map(group => `
    <li>${group.id} — ${escapeHtml(group.name)}</li>
  `).join("");
}

async function loadStudents() {
  const response = await fetch(studentsUrl);
  if (!response.ok) throw new Error("Не удалось загрузить студентов");
  const students = await response.json();

  document.querySelector("#students").innerHTML = students.map(student => `
    <tr>
      <td>${student.id}</td>
      <td>${escapeHtml(student.firstName)}</td>
      <td>${escapeHtml(student.lastName)}</td>
      <td>${student.groupId ?? "—"}</td>
    </tr>
  `).join("");
}

document.querySelector("#student-form").addEventListener("submit", async event => {
  event.preventDefault();
  const message = document.querySelector("#message");
  message.textContent = "";

  const payload = {
    firstName: document.querySelector("#firstName").value.trim(),
    lastName: document.querySelector("#lastName").value.trim(),
    groupId: document.querySelector("#groupId").value || null
  };

  try {
    const response = await fetch("/students/add", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      const result = await response.json().catch(() => ({}));
      throw new Error(result.message || "Не удалось добавить студента");
    }

    event.target.reset();
    await loadStudents();
  } catch (error) {
    message.textContent = error.message;
  }
});

document.querySelector("#group-form").addEventListener("submit", async event => {
  event.preventDefault();
  const message = document.querySelector("#message");
  message.textContent = "";

  const payload = { name: document.querySelector("#groupName").value.trim() };

  try {
    const response = await fetch("/groups/add", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      const result = await response.json().catch(() => ({}));
      throw new Error(result.message || "Не удалось добавить группу");
    }

    event.target.reset();
    await loadGroups();
  } catch (error) {
    message.textContent = error.message;
  }
});

function escapeHtml(value) {
  return String(value ?? "").replace(/[&<>"']/g, character => ({
    "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
  }[character]));
}

Promise.all([loadGroups(), loadStudents()]).catch(error => {
  document.querySelector("#message").textContent = error.message;
});
