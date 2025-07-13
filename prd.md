---
title: Cricut Stencil Maker (Windows)
description: Drag-and-drop images ➜ instant, weed-friendly, background-removed SVG stencils for Cricut Design Space
version: 0.9.0 - Draft
---

## 1  Why This App Exists
- **One-click stencil workflow.** Manual tracing or Illustrator/Inkscape cleanup is slow; this app automates bitmap ➜ cut-ready SVG in seconds.
- **Weed-friendly output.** Prevents micro-islands and adds optional *bridges* so stencil centers stay put on mylar or cardstock.
- **Full background removal.** No need for Cricut’s Erase/Remove. Includes auto subject segmentation + manual refine brush.
- **Design Space compatibility.** Exports *plain SVG* with only `<path>` elements; ≤5000 paths to avoid upload errors.

## 2  Key Capabilities
| Feature                | Details |
|-------------------------|---------|
| **File types**          | PNG · JPEG · BMP · TIFF · GIF · WebP ➜ auto-detected & converted |
| **Background removal**  | AI segmentation + manual erase/restore brush. |
| **Ultra-fast processing** | C++17 core with AVX-512 + GPU (DirectML) acceleration; ~200 ms for 2048×2048px |
| **Thresholding**        | Adaptive, Otsu, manual override for sharp edges |
| **Vectorization**       | Centerline + outline modes; Potrace-like precision |
| **Path simplification** | Ramer-Douglas-Peucker + angle filter, ≤4000 nodes, ≤5000 paths |
| **Weeding optimizer**   | Removes micro-paths below X mm², merges shapes, adds *weeding boxes* |
| **Bridge generator**    | Auto-detects enclosed “islands” and inserts adjustable-width bridges |
| **Preset sizes**        | 11.5×11.5 in, 23.5×11.5 in, custom with locked aspect ratio |
| **Preview modes**       | Cut lines · Weed view · Bridge view · Background-removed overlay |
| **Batch + CLI**         | `cricut-stencil --input *.png --background-remove --size 11in` |
| **Localized**           | en-US, es-ES, de-DE (resx-based UI strings) |

## 3  Typical Workflow
1. **Drag-and-drop** any image.
2. **Remove background** automatically; refine with brush if needed.
3. **Choose material preset**  
   - *Vinyl / HTV* ➜ focus on weed reduction.  
   - *Mylar* ➜ enable bridges, set width.  
4. **Adjust sliders** (optional)  
   - Detail %, Path smoothness, Min island size, Bridge spacing.  
5. **Preview** in Weed/Bridge/Cut mode.  
6. **Export Plain SVG**; ready for Design Space import.

## 4  How We Solve Common Design Space Pain Points
| DS Problem                                   | Our Solution                                        |
|----------------------------------------------|-----------------------------------------------------|
| *“Unsupported items”* (clipPaths, text)      | Exports flattened `<path>`-only SVG. |
| *“File Too Large – reduce size”* (>5000 paths) | Enforced node/path limit with pre-export warnings. |
| Incomplete background removal in DS          | Full AI subject extraction + manual refine. |
| Tedious weeding                              | Minimum island filter + optional weeding boxes. |
| Interior pieces fall out of stencil          | Automatic bridge generator with adjustable width. |
| Wrong mat size                               | ViewBox + presets for 11.5 or 23.5 in; autoscale if oversized. |
| Negative coordinates shifting image          | All paths translated to positive space. |
| Duplicate-path rendering bugs                | Path deduplication + direction normalization. |

## 5  Design Best Practices
- **Drag-and-Drop Simplicity.** No complex menus. User drops an image; instant preview with background removed and vector paths.  
- **Auto-Inkscape Steps:**  
  - Bitmap Trace ➜ automated.  
  - Object to Path ➜ guaranteed.  
  - Path ▸ Simplify ➜ auto ≤4000 nodes.  
  - Add bridges ➜ auto-detected, adjustable.  
  - Document properties ➜ preset mat sizes in inches.  
  - Export ➜ always Plain SVG.  
- **Background Removal UI:**  
  - Automatic on import.  
  - Manual brush for precision.  
  - Edge smoothness slider.  
- **Live Preview Modes:**  
  - Cut lines.  
  - Weed view.  
  - Bridge view.  
  - Transparency overlay.  
- **Mat-Sized Export:**  
  - Auto scales to 11.5×11.5 or 23.5×11.5 in.  
  - Warns if oversized.  
- **Error-Proof Export:**  
  - ≤5000 paths.  
  - Deduplication.  
  - Positive coordinates.  
- **Smart Defaults & Recommendations:**  
  - Auto-selects best settings per image.  
  - No need for technical knowledge.  
  - Tooltips and recommendations if result needs tweaking.  
  - One-click “Improve This” to re-run with tuned settings.  
  - Default workflow delivers *good results immediately*, always tweakable.

## 6  Easy Install Experience
- **Simple Installer:**  
  - One-click .exe installer with Windows-standard wizard.  
  - No command-line needed.  
  - Automatic start menu and desktop shortcuts.  
  - Signed installer for security prompts.  
  - Supports auto-updates or manual check-for-updates.  
  - Minimal footprint < 30 MB.  
- **No Dependencies:**  
  - Fully bundled runtime (static CRT).  
  - Works on Windows 10+ without .NET installs.  
  - No admin rights needed for user-space install option.

## 7  Architecture & Performance Notes
- **Language/Framework:** C++17 core with WinUI 3; Intel® SVML vector ops; GPU via DirectML.
- **Memory-safe containers**, thread pooling for huge images.
- **Async pipeline**: decode ➜ background removal ➜ threshold ➜ vectorize ➜ path cleanup.
- **Installer footprint:** < 30 MB, static CRT, no .NET required.

## 8  Roadmap
- **Auto-color separation** for multi-layer HTV (Q4-25).  
- **Direct Cricut Maker 4 send via Bluetooth** once DS API matures.  
- **Stencil font suggestion** when text is detected.  
- **Open-source plugin SDK** for alternative vectorizers or dithering.


