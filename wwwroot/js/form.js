namespace collegeAdmission.wwwroot.js
{
    public class form
    {
    }
    document.addEventListener("DOMContentLoaded", () => {
        const programSelect = document.getElementById("program");
        const courseSelect = document.getElementById("course");
        const ugMarksheetGroup = document.getElementById("ugMarksheetGroup");

        const courses = {
            UG: ["B.Sc Computer Science", "B.A English", "B.Com Commerce", "B.Tech Engineering"],
            PG: ["M.Sc Computer Science", "M.A English", "M.Com Commerce", "M.Tech Engineering"]
        };

        programSelect.addEventListener("change", () => {
            const program = programSelect.value;

            // Update course dropdown
            courseSelect.innerHTML = "<option value='' disabled selected>Select Course</option>";
            if (courses[program]) {
                courses[program].forEach(course => {
                    const option = document.createElement("option");
                    option.value = course;
                    option.textContent = course;
                    courseSelect.appendChild(option);
                });
            }

            // Show or hide UG Marksheet upload field
            if (program === "PG") {
                ugMarksheetGroup.style.display = "block";
            } else {
                ugMarksheetGroup.style.display = "none";
            }
        });

        document.getElementById("admissionForm").addEventListener("submit", (e) => {
            e.preventDefault();
            alert("Form submitted successfully!");
        });
    });

}
